﻿using FluentAssertions;
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
    public class ManipulatorTests: RoleTestFunctions
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
    }
}
