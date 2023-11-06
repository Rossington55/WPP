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
    public class ManipulatedTests: RoleTests
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

    }
}
