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
    public class RoleTestFunctions
    {
        public readonly Game game;
        public Message serverMessage;
        public Message nightInfoMessage;
        public RoleTestFunctions()
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



    }
}
