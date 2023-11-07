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
    public class Villager : RoleTestFunctions
    {
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

        [Fact]
        public void Tanner()
        {
            List<Connection> players = CreatePlayers(1);
            game.Start(players, "Custom;Tanner");
            Player tanner = game.GetPlayerByRole("Tanner");

            SetServerMessage(tanner.name, tanner.name);
            serverMessage.commandServer = CommandServer.SubmitVote;
            game.Update(serverMessage);
            var result = game.CheckEndgame(false);

            result.Should().Be(Team.Tanner);
        }

        [Theory]
        [InlineData(2)]//Bulldoze vote
        [InlineData(3)]//Tie vote
        public void Mayor(int playerCount)
        {
            List<Connection> players = CreatePlayers(playerCount);
            string gameMode = "Custom;Mayor;Villager";
            if (playerCount == 3) { gameMode += ";Seer"; }
            game.Start(players, gameMode);
            Player mayor = game.GetPlayerByRole("Mayor");
            Player villager = game.GetPlayerByRole("Villager");
            Player seer = game.GetPlayerByRole("Seer");

            //Mayor votes for villager - no majority
            SetServerMessage(mayor.name, villager.name);
            serverMessage.commandServer = CommandServer.SubmitVote;
            game.Update(serverMessage);

            if (playerCount == 2)
            {
                villager.alive.Should().BeFalse();
                return;
            }
            else
            {
                villager.alive.Should().BeTrue();
            }

            //Villager and seer vote Mayor - tie vote
            SetServerMessage(villager.name, mayor.name);
            game.Update(serverMessage);
            SetServerMessage(seer.name, mayor.name);
            game.Update(serverMessage);
            villager.alive.Should().BeTrue();
            mayor.alive.Should().BeTrue();
        }
    }
}
