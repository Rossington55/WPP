using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Werewolf_Server;
using Werewolf_Server.GameFiles.Roles.Active;

namespace WerewolfServerTest.Tests
{
    public class RoleTests
    {
        public readonly Game game;
        public Message serverMessage;
        public Message nightInfoMessage;
        public RoleTests()
        {
            game = new Game();
            serverMessage = new Message("", CommandServer.NightSubmit, "");
        }

        public void SetServerMessage(string playerName, string data)
        {
            serverMessage.player = playerName;
            serverMessage.data[0] = data;
        }
        public void SetServerMessage(string playerName, string data, string subCommand)
        {
            serverMessage.player = playerName;
            serverMessage.data[0] = data;
            serverMessage.subCommand = subCommand;
        }

        public List<Connection> CreatePlayers(int playerCount)
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

        public bool GetNightMessage(List<Message> result)
        {
            nightInfoMessage = result.Find(msg => msg.commandClient == CommandClient.Submitted);
            nightInfoMessage.Should().NotBeNull();

            return nightInfoMessage != null;
        }

        public void InitGameForNight(int playerCount, string gameMode)
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



    }
}
