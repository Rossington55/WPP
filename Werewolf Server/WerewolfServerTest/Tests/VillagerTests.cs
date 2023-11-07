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
    public class Villager: RoleTestFunctions
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

    }
}
