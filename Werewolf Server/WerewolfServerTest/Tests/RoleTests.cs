﻿using FluentAssertions;
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
        public RoleTests()
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

        private void InitGameForNight(int playerCount, string gameMode)
        {
            List<Connection> players = CreatePlayers(playerCount);
            game.Start(players,gameMode);

            Message message = new Message("", CommandServer.StartNight, "");
            game.Update(message);
        }

        [Fact]
        public void Villager_Submit()
        {
            int playerCount = 3;
            InitGameForNight(playerCount,"VillagerOnly");
            Message message = new Message(
                "0",
                CommandServer.NightSubmit,
                ""
                );

            var result = game.Update(message);

            //Submitted, update host, update all players of day
            result.Should().HaveCountGreaterThan(2);
            if(result.Count == 0) { return; }

            result[0].commandClient.Should().Be(CommandClient.Submitted);

            //Villagers only, should all be ready
            result[1].commandClient.Should().Be(CommandClient.State);
            result[1].data[0].Should().Be(State.Day.ToString());
        }

        [Fact]
        public void Werewolf_Submit()
        {
            int playerCount = 3;
            InitGameForNight(playerCount, "Mafia");
            Player werewolf = game.GetPlayerByRole("Werewolf");
            werewolf.Should().NotBeNull();

            Message biteMessage = new Message(werewolf.name, CommandServer.WerewolfSelectPlayer, "0");
            biteMessage.subCommand = "select";
            Message submitMessage = new Message(werewolf.name, CommandServer.NightSubmit, "");

            //Bite a playerd
            game.Update(biteMessage);


            var result = game.Update(submitMessage);

            //Submitted, update host, update all players of day, murdered
            result.Should().HaveCountGreaterThan(3);
            if(result.Count == 0) { return; }

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
    }
}