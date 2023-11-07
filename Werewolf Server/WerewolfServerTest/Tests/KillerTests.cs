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
    public class KillerTests: RoleTestFunctions
    {
        [Fact]
        public void Huntress_Submit()
        {
            InitGameForNight(2, "Custom;Huntress;Villager");
            Player huntress = game.GetPlayerByRole("Huntress");
            Player labRat = game.GetPlayerByRole("Villager");

            SetServerMessage(huntress.name, labRat.name);
            game.Update(serverMessage);
            game.FinishNight();

            labRat.alive.Should().BeFalse();
        }
    }
}
