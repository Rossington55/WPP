using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Werewolf_Server;

namespace WerewolfServerTest.Tests
{
    public class RoleTests
    {
        private readonly Game game;
        private Message serverMessage;
        private Message nightInfoMessage;
        public RoleTests()
        {
            game = new Game();
            serverMessage = new Message("", CommandServer.NightSubmit, "");
        }

        private void SetServerMessage(string playerName, string data)
        {
            serverMessage.player = playerName;
            serverMessage.data[0] = data;
        }
        private void SetServerMessage(string playerName, string data, string subCommand)
        {
            serverMessage.player = playerName;
            serverMessage.data[0] = data;
            serverMessage.subCommand = subCommand;
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

        private bool GetNightMessage(List<Message> result)
        {
            nightInfoMessage = result.Find(msg => msg.commandClient == CommandClient.Submitted);
            nightInfoMessage.Should().NotBeNull();

            return nightInfoMessage != null;
        }

        private void InitGameForNight(int playerCount, string gameMode)
        {
            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players, gameMode);

            Message message = new Message("", CommandServer.StartNight, "");
            game.Update(message);
        }

        [Fact]
        public void Villager_Submit()
        {
            InitGameForNight(3, "VillagerOnly");
            SetServerMessage("0", "");

            var result = game.Update(serverMessage);

            //Submitted, update host, update all players of day
            result.Should().HaveCount(0);
        }

        [Fact]
        public void Werewolf_Submit()
        {
            InitGameForNight(3, "Custom;Villager;Villager;Werewolf");
            Player werewolf = game.GetPlayerByRole("Werewolf");
            werewolf.Should().NotBeNull();

            Message biteMessage = new Message(werewolf.name, CommandServer.WerewolfSelectPlayer, "0");
            biteMessage.subCommand = "select";
            SetServerMessage(werewolf.name, "");

            //Bite a playerd
            game.Update(biteMessage);


            var result = game.Update(serverMessage);

            //Submitted, update host, update all players of day, murdered
            result.Should().HaveCountGreaterThan(3);
            if (result.Count == 0) { return; }

            result[0].commandClient.Should().Be(CommandClient.Submitted);

            //Only waiting for me, should all be ready to go to day
            Message? dayMessage = result.Find(msg => msg.commandClient == CommandClient.State);
            dayMessage.Should().NotBeNull();
            dayMessage.data[0].Should().Be(State.Day.ToString());

            //Someone was bitten
            //and that person is correct
            Message murderMessage = result.Find(message => message.commandClient == CommandClient.Murdered);
            murderMessage.Should().NotBeNull();
            murderMessage.player.Should().Be("0");
        }

        [Theory]
        [InlineData("Werewolf", true)]
        [InlineData("Lycan", true)]
        [InlineData("Villager", false)]
        [InlineData("Seer", false)]
        public void Seer_Submit(string selectedRole, bool expectedWerewolf)
        {
            InitGameForNight(4, "Custom;Seer;Werewolf;Villager;Lycan");
            Player seer = game.GetPlayerByRole("Seer");
            Player selectedPlayer = game.GetPlayerByRole(selectedRole);
            seer.Should().NotBeNull();

            SetServerMessage(seer.name, selectedPlayer.name);
            var result = game.Update(serverMessage);

            if (!GetNightMessage(result)) { return; }
            if (expectedWerewolf)
            {
                nightInfoMessage.data[0].Should().Contain("IS");
            }
            else
            {
                nightInfoMessage.data[0].Should().Contain("NOT");
            }
        }

        [Fact]
        public void ApprenticeSeer_Submit()
        {
            InitGameForNight(4, "Custom;Seer;Apprentice Seer;Werewolf;Villager");
            Player seer = game.GetPlayerByRole("Seer");
            Player apprentice = game.GetPlayerByRole("Apprentice Seer");
            Player werewolf = game.GetPlayerByRole("Werewolf");


            //Submit while Seer is still alive
            SetServerMessage(apprentice.name, "0");

            var result = game.Update(serverMessage);

            //Kill the Seer
            //Ready up everyone to finish the night
            foreach (Player player in game.AlivePlayers)
            {
                player.ready = true;
            }
            seer.werewolvesAttacking++;
            SetServerMessage(werewolf.name, seer.name);
            game.Update(serverMessage);

            //Submit while Seer is still dead
            SetServerMessage(apprentice.name, werewolf.name);
            result = game.Update(serverMessage);
            if (!GetNightMessage(result)) { return; }
            nightInfoMessage.data[0].Should().Contain("Werewolf");

        }

        [Theory]
        [InlineData("Health")]
        [InlineData("Poison")]
        public void Witch_Submit(string potion)
        {
            InitGameForNight(2, "Custom;Witch;Villager");
            Player witch = game.GetPlayerByRole("Witch");
            Player labRat = game.GetPlayerByRole("Villager");

            for (int i = 0; i < 2; i++)
            {
                if (potion == "Health")
                {
                    //Bite the lab rat
                    labRat.werewolvesAttacking++;
                }

                SetServerMessage(witch.name, labRat.name, potion);
                game.Update(serverMessage);

                game.FinishNight();

                if (potion == "Health")
                {
                    if (i == 0)
                    {
                        labRat.alive.Should().BeTrue();
                    }
                    else//Dont work on second use
                    {
                        labRat.alive.Should().BeFalse();

                    }
                }
                else if (potion == "Poison")
                {
                    if (i == 0)
                    {
                        labRat.alive.Should().BeFalse();
                    }
                    else//Dont work on second use
                    {
                        labRat.alive.Should().BeTrue();

                    }
                }

                //Revive the rat
                labRat.alive = true;
                labRat.deathTimer = -1;
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public void Mason_Submit(int masonCount)
        {
            //Populate game with required masons
            string roles = "Custom";
            for (int i = 0; i < masonCount; i++)
            {
                roles += ";Mason";
            }
            InitGameForNight(masonCount + 1, roles);
            Player mason = game.GetPlayerByRole("Mason");

            SetServerMessage(mason.name, "");

            var result = game.Update(serverMessage);
            if (!GetNightMessage(result)) { return; }
            nightInfoMessage.data.Should().HaveCount(masonCount);//Correct amount of other masons
            nightInfoMessage.data.Should().NotContain(otherMason => otherMason == mason.name);

        }

        [Theory]
        [InlineData("Werewolf", true)]
        [InlineData("Villager", false)]
        [InlineData("Seer", false)]
        public void Revealer_Submit(string findRole, bool theyDie)
        {
            InitGameForNight(4, "Custom;Revealer;Werewolf;Villager;Seer");
            Player revealer = game.GetPlayerByRole("Revealer");
            Player selectedPlayer = game.GetPlayerByRole(findRole);
            SetServerMessage(revealer.name, selectedPlayer.name);


            game.Update(serverMessage);
            game.FinishNight();

            if (theyDie)
            {
                revealer.alive.Should().BeTrue();
                selectedPlayer.alive.Should().BeFalse();
            }
            else
            {
                revealer.alive.Should().BeFalse();
                selectedPlayer.alive.Should().BeTrue();
            }
        }

        [Fact]
        public void Priest_Submit()
        {
            InitGameForNight(2, "Custom;Priest;Villager");
            Player priest = game.GetPlayerByRole("Priest");
            Player labRat = game.GetPlayerByRole("Villager");

            for (int i = 0; i < 2; i++)
            {
                //Bite the lab rat
                labRat.werewolvesAttacking++;

                SetServerMessage(priest.name, labRat.name);
                game.Update(serverMessage);

                game.FinishNight();

                if (i == 0)
                {
                    labRat.alive.Should().BeTrue();
                }
                else//Dont work on second use
                {
                    labRat.alive.Should().BeFalse();

                }

                //Revive the rat
                labRat.alive = true;
                labRat.deathTimer = -1;
            }
        }

        [Fact]
        public void ToughGuy()
        {
            InitGameForNight(1, "Custom;Tough Guy");
            Player toughGuy = game.GetPlayerByRole("Tough Guy");

            //Dont die first time
            toughGuy.werewolvesAttacking++;
            game.FinishNight();
            toughGuy.alive.Should().BeTrue();

            //Die second night
            game.NightInit();
            game.FinishNight();
            toughGuy.alive.Should().BeFalse();

        }
    }
}
