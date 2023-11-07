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
    public class ManipulatorTests : RoleTestFunctions
    {
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

        [Fact]
        public void CultLeader()
        {
            InitGameForNight(4, "Custom;Cult Leader;Villager;Seer;Mason");
            Player cultLeader = game.GetPlayerByRole("Cult Leader");
            Player villager = game.GetPlayerByRole("Villager");
            Player seer = game.GetPlayerByRole("Seer");
            Player mason = game.GetPlayerByRole("Mason");
            Team winningTeam;

            //By end of night - Cult,Villager(cult),Seer,Mason(dead) - no win
            mason.deathTimer = 0;
            SetServerMessage(cultLeader.name, villager.name);
            game.Update(serverMessage);
            game.FinishNight();
            winningTeam = game.CheckEndgame(false);
            winningTeam.Should().NotBe(Team.Cult);

            //By end of night - Cult,Villager(cult),Seer(cult),Mason(dead) - win!
            SetServerMessage(cultLeader.name, seer.name);
            game.Update(serverMessage);
            game.FinishNight();
            winningTeam = game.CheckEndgame(false);
            winningTeam.Should().Be(Team.Cult);
        }

        [Fact]
        public void Diseased()
        {
            InitGameForNight(2, "Custom;Werewolf;Diseased");
            Player diseased = game.GetPlayerByRole("Diseased");
            Player werewolf = game.GetPlayerByRole("Werewolf");

            //Kill but not by werewolf
            diseased.deathTimer = 0;
            game.FinishNight();

            diseased.alive.Should().Be(false);
            werewolf.role.hasNightTask.Should().BeTrue();

            //Revive diseased
            diseased.alive = true;
            diseased.deathTimer = -1;

            //Kill by werewolf
            diseased.werewolvesAttacking++;
            game.FinishNight();

            diseased.alive.Should().Be(false);
            werewolf.role.hasNightTask.Should().BeFalse();

            //Wait a night
            game.FinishNight();
            werewolf.role.hasNightTask.Should().BeTrue();
        }

        [Theory]
        [InlineData("Villager", "Seer")]
        [InlineData("Seer", "Seer")]
        public void OldHag_Submit(string player1Role, string player2Role)
        {
            InitGameForNight(3, "Custom;Old Hag;Villager;Seer");
            Player oldHag = game.GetPlayerByRole("Old Hag");
            Player player1 = game.GetPlayerByRole(player1Role);
            Player player2 = game.GetPlayerByRole(player2Role);

            //First night
            SetServerMessage(oldHag.name, player1.name);
            var result = game.Update(serverMessage);
            result.Should().Contain(message => message.commandClient == CommandClient.Alert);
            player1.canVote.Should().BeFalse();

            //Check unable to vote
            SetServerMessage(player1.name, player2.name);
            serverMessage.commandServer = CommandServer.SelectVote;
            result = game.Update(serverMessage);
            result.Should().HaveCount(0);
            serverMessage.commandServer = CommandServer.SubmitVote;
            result = game.Update(serverMessage);
            result.Should().HaveCount(0);

            //Second night
            SetServerMessage(oldHag.name, player2.name);
            serverMessage.commandServer = CommandServer.NightSubmit;
            game.NightInit();
            result = game.Update(serverMessage);
            if (player1Role == player2Role)
            {
                player2.canVote.Should().BeTrue();
            }
            else
            {
                result.Should().Contain(message => message.commandClient == CommandClient.Alert);
                player2.canVote.Should().BeFalse();
            }

        }

        [Theory]
        [InlineData("Villager", "Seer")]
        [InlineData("Seer", "Seer")]
        public void Spellcaster_Submit(string player1Role, string player2Role)
        {
            InitGameForNight(3, "Custom;Spellcaster;Villager;Seer");
            Player oldHag = game.GetPlayerByRole("Spellcaster");
            Player player1 = game.GetPlayerByRole(player1Role);
            Player player2 = game.GetPlayerByRole(player2Role);

            //First night
            SetServerMessage(oldHag.name, player1.name);
            var result = game.Update(serverMessage);
            result.Should().Contain(message => message.commandClient == CommandClient.Alert);

            //Second night
            SetServerMessage(oldHag.name, player2.name);
            serverMessage.commandServer = CommandServer.NightSubmit;
            game.NightInit();
            result = game.Update(serverMessage);
            if (player1Role == player2Role)
            {
                result.Should().NotContain(message => message.commandClient == CommandClient.Alert);
            }
            else
            {
                result.Should().Contain(message => message.commandClient == CommandClient.Alert);
            }
        }

        [Fact]
        public void Cupid_Submit()
        {
            InitGameForNight(3, "Custom;Cupid;Villager;Seer");
            Player cupid = game.GetPlayerByRole("Cupid");
            Player player1 = game.GetPlayerByRole("Villager");
            Player player2 = game.GetPlayerByRole("Seer");

            //Link player 1 and player 2
            SetServerMessage(cupid.name, "");
            serverMessage.data = new List<string>() { player1.name, player2.name };
            game.Update(serverMessage);

            //Kill player 1
            player1.werewolvesAttacking++;
            game.FinishNight();

            player1.alive.Should().BeFalse();
            player2.alive.Should().BeFalse();
        }
    }
}
