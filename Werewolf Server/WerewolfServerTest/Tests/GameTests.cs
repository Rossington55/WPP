using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Werewolf_Server;
using Werewolf_Server.GameFiles;

namespace WerewolfServerTest.Tests
{
    public class GameTests
    {
        private readonly Game game;
        public GameTests()
        {
            game = new Game();
        }

        private List<Connection> CreatePlayers(int playerCount)
        {
            List<Connection> connections = new List<Connection>();
            for (int i = 0; i < playerCount; i++)
            {
                connections.Add(new Connection(
                    i.ToString(),
                    null
                    ));
            }

            return connections;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(10)]
        public void Game_Start(int userCount)
        {
            List<Connection> connections = new List<Connection>();
            for (int i = 0; i < userCount; i++)
            {
                connections.Add(new Connection(i.ToString(), null));
            }

            var result = game.Start(connections, "");

            result.Should().BeOfType<List<Message>>();
            result.Should().HaveCountGreaterThan(userCount);
        }

        [Theory]
        [InlineData(CommandServer.Join)]
        public void Game_Update_Default(CommandServer command)
        {
            Message message = new Message("", command, "");

            var result = game.Update(message);

            result.Should().BeOfType<List<Message>>();
            result.Count().Should().Be(0);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        public void Game_Update_NightInit(int playerCount)
        {
            Message message = new Message(
            "",
            CommandServer.StartNight,
            ""
                );

            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players, "");

            var result = game.Update(message);

            //Verify messages
            result.Count.Should().Be(1 + playerCount);//Host + (Alive players and state)
            result[0].data.Should().BeEquivalentTo(State.Night.ToString());//First message is state to host

            if (result.Count > 1)
            {
                result[1].commandClient.Should().Be(CommandClient.State);
            }

            //Verify Game
            game.state.Should().Be(State.Night);
        }

        [Theory]
        [InlineData("1", true)]//Should succeed
        [InlineData("1", false)]//Should succeed
        [InlineData("I dont exist", true)]//Invalid player
        [InlineData("I dont exist", false)]//Invalid player
        [InlineData("", true)]//No player selected
        [InlineData("", false)]//No player selected
        public void Game_Update_WWClick(string selectedPlayer, bool isSelecting)
        {
            Message message = new Message(
                "0",//From player 0
                CommandServer.WerewolfSelectPlayer,
                selectedPlayer
                );
            if (isSelecting)
            {
                message.subCommand = "select";
            }
            else
            {
                message.subCommand = "deselect";
            }

            //Init the game
            int playerCount = 3;
            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players, "Custom;Werewolf;Werewolf;Sorceress");

            var result = game.Update(message);

            //There is a result for a valid player
            if (selectedPlayer == "1")
            {
                result.Should().HaveCount(playerCount-1);//Shouldnt message sorceress
                if (result.Count == 0) { return; }
                //Correct command
                result[0].commandClient.Should().Be(CommandClient.SelectedPlayerList);

                //Correct data
                result[0].data.Should().HaveCountGreaterThan(0);
                if (result[0].data.Count == 0) { return; }

                if (isSelecting)
                {
                    result[0].data.Should().Contain(item => item.Contains(";1"));
                }
                else
                {
                    result[0].data.Should().NotContain(item => item.Contains(";-1"));
                    result[0].data.Should().NotContain(item => item.Contains(";-1"));
                }

            }
            else
            {
                result.Should().HaveCount(0);
            }
        }

        [Theory]
        [InlineData("1", true)]//Should succeed
        [InlineData("1", false)]//Should succeed
        [InlineData("I dont exist", true)]//Invalid player
        [InlineData("I dont exist", false)]//Invalid player
        [InlineData("", true)]//No player selected
        [InlineData("", false)]//No player selected
        public void Game_Update_SelectVote(string selectedPlayer, bool isSelecting)
        {
            Message message = new Message(
                "0",//From player 0
                CommandServer.SelectVote,
                selectedPlayer
                );
            if (isSelecting)
            {
                message.subCommand = "select";
            }
            else
            {
                message.subCommand = "deselect";
            }

            //Init the game
            int playerCount = 3;
            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players, "");

            var result = game.Update(message);

            //There is a result for a valid player
            if (selectedPlayer == "1")
            {
                result.Should().HaveCountGreaterThan(0);
                if (result.Count == 0) { return; }
                //Correct command
                result[0].commandClient.Should().Be(CommandClient.SelectedPlayerList);

                //Correct data
                result[0].data.Should().HaveCountGreaterThan(0);
                if (result[0].data.Count == 0) { return; }

                if (isSelecting)
                {
                    result[0].data.Should().Contain(item => item.Contains(";1"));
                }
                else
                {
                    result[0].data.Should().NotContain(item => item.Contains(";-1"));
                    result[0].data.Should().NotContain(item => item.Contains(";-1"));
                }
            }
            else
            {
                result.Should().HaveCount(0);
            }
        }

        [Theory]
        [InlineData("1")]
        [InlineData("I dont exist")]
        [InlineData("")]
        public void Game_Update_SubmitVote(string selectedPlayer)
        {
            Message message = new Message(
                "0",//From player 0
                CommandServer.SubmitVote,
                selectedPlayer
                );

            //Init the game
            int playerCount = 3;
            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players, "");

            //Check that voting works
            var result = game.Update(message);
            if (selectedPlayer != "1")
            {
                result.Should().HaveCount(1);
                return;
            }

            result.Should().HaveCountGreaterThan(1);
            if (result.Count == 0) { return; }

            result[0].commandClient.Should().Be(CommandClient.Submitted);
        }

        [Fact]
        public void Game_Update_SubmitVote_Majority()
        {
            string selectedPlayer = "0";
            Message message = new Message(
                "0",
                CommandServer.SubmitVote,
                selectedPlayer
                );

            //Init the game
            int playerCount = 3;
            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players, "");

            //First vote - no majority
            var result = game.Update(message);
            result.Should().HaveCountLessThan(3);

            //Second vote - majority
            message.player = "1";
            result = game.Update(message);
            result.Should().HaveCountGreaterThan(2);
            result[2].commandClient.Should().Be(CommandClient.Murdered);   
            result[2].player.Should().Be(selectedPlayer);   
        }

        [Fact]
        public void Game_Update_SubmitVote_SplitVote()
        {
            Message message = new Message(
                "0",
                CommandServer.SubmitVote,
                "0"
                );

            //Init the game
            int playerCount = 2;
            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players, "");

            //First vote - no majority
            message.data[0] = "0";//Select player 0
            var result = game.Update(message);
            result.Should().HaveCountLessThan(3);

            //Second vote - no majority
            message.data[0] = "1";//Select player 1
            message.player = "1";
            result = game.Update(message);
            result.Should().HaveCountGreaterThan(2);
            result[2].commandClient.Should().NotBe(CommandClient.Murdered);   
        }

        [Fact]
        public void Game_Update_SubmitVote_PreferenceVote()
        {
            Message message = new Message(
                "0",
                CommandServer.SubmitVote,
                "0"
                );

            //Init the game
            int playerCount = 6;
            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players, "");

            //Vote 1 - p0:1
            message.data[0] = "0";//Select player 0
            var result = game.Update(message);
            result.Should().HaveCountLessThan(3);

            //Vote 2 - p0:2
            message.player = "1";
            result = game.Update(message);
            result.Should().HaveCountLessThan(3);

            //Vote 3 - p0:3, 
            message.player = "2";
            result = game.Update(message);
            result.Should().HaveCountLessThan(3);

            //Vote 4 - p0:3, p1:1
            message.player = "3";
            message.data[0] = "1";//Select player 0
            result = game.Update(message);
            result.Should().HaveCountLessThan(3);

            //Vote 5 - p0:3, p1:2
            message.player = "4";
            message.data[0] = "2";//Select player 0
            result = game.Update(message);
            result.Should().HaveCountLessThan(3);

            //Vote 6 - FINAL - p0:3, p1:2, p2:1
            message.player = "5";
            message.data[0] = "3";//Select player 0
            result = game.Update(message);
            result.Should().HaveCountGreaterThan(2);
            result[2].commandClient.Should().Be(CommandClient.Murdered);   
        }

        [Fact]
        public void Game_CheckEndgame_NoWin()
        {
            //Init the game
            int playerCount = 3;
            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players, "Mafia");

            var result = game.CheckEndgame(false);
            result.Should().Be(Team.None);
        }

        [Fact]
        public void Game_CheckEndgame_VillagerWin()
        {
            //Init the game
            int playerCount = 3;
            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players, "VillagerOnly");

            var result = game.CheckEndgame(false);
            result.Should().Be(Team.Villager);
        }

        [Fact]
        public void Game_CheckEndgame_WerewolfWin()
        {
            //Init the game
            int playerCount = 3;
            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players, "Custom;Werewolf;Werewolf;Villager");

            var result = game.CheckEndgame(false);
            result.Should().Be(Team.Werewolf);
        }
    }
}
