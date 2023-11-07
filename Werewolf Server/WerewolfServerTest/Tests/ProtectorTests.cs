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
    public class ProtectorTests : RoleTestFunctions
    {
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
        public void Bodyguard_Submit()
        {
            InitGameForNight(3, "Custom;Bodyguard;Villager;Seer");
            Player bodyguard = game.GetPlayerByRole("Bodyguard");
            Player labRat = game.GetPlayerByRole("Villager");
            Player seer = game.GetPlayerByRole("Seer");

            for (int i = 0; i < 4; i++)
            {
                //Bite the lab rat

                SetServerMessage(bodyguard.name, labRat.name);

                if (i == 2)
                {
                    SetServerMessage(bodyguard.name, seer.name);
                    seer.werewolvesAttacking = 1;
                    labRat.werewolvesAttacking = 0;

                }
                else
                {
                    seer.werewolvesAttacking = 0;
                    labRat.werewolvesAttacking = 1;
                }
                game.Update(serverMessage);

                game.FinishNight();

                switch (i)
                {

                    case 0:
                        labRat.alive.Should().BeTrue();
                        break;
                    case 1:
                        labRat.alive.Should().BeFalse();
                        break;
                    case 2:
                        labRat.alive.Should().BeTrue();
                        seer.alive.Should().BeTrue();
                        break;
                    case 3:
                        labRat.alive.Should().BeTrue();
                        seer.alive.Should().BeTrue();
                        break;
                }

                //Revive the rat
                labRat.alive = true;
                labRat.deathTimer = -1;
                seer.alive = true;
            }
        }
    }
}
