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
    public class ManipulatedTests : RoleTestFunctions
    {


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

        [Fact]
        public void Cursed()
        {
            InitGameForNight(1, "Custom;Cursed");
            Player cursed = game.GetPlayerByRole("Cursed");

            //Starts cursed
            cursed.role.name.Should().Be("Cursed");
            cursed.role.team.Should().Be(Team.Villager);

            //Bite them
            cursed.werewolvesAttacking++;
            game.FinishNight();
            cursed.alive.Should().BeTrue();
            cursed.role.team.Should().Be(Team.Werewolf);
        }

        [Fact]
        public void Drunk()
        {
            InitGameForNight(1, "Custom;Drunk");
            Player drunk = game.GetPlayerByRole("Drunk");

            for (int i = 0; i < 3; i++)
            {
                SetServerMessage(drunk.name, "");
                var result = game.Update(serverMessage);

                if (i == 2)//On third night
                {
                    result.Should().Contain(message => message.commandClient == CommandClient.Role);
                    drunk.role.name.Should().NotBe("Drunk");
                }
                else
                {
                    result.Should().NotContain(message => message.commandClient == CommandClient.Role);
                }
            }
        }

        [Fact]
        public void Doppelganger()
        {
            InitGameForNight(3, "Custom;Doppelganger;Seer;Villager");
            Player doppelganger = game.GetPlayerByRole("Doppelganger");
            Player seer = game.GetPlayerByRole("Seer");
            Player villager = game.GetPlayerByRole("Villager");

            SetServerMessage(doppelganger.name, seer.name);
            game.Update(serverMessage);

            //First night - doppelganger in waiting
            villager.deathTimer = 0;
            game.FinishNight();
            doppelganger.role.hasNightTask.Should().BeFalse();

            //Second night - doppelganger phases
            seer.deathTimer = 0;
            game.FinishNight();
            doppelganger.role.name.Should().Be("Seer");

        }

    }
}
