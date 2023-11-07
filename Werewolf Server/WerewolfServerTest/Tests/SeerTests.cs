using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Werewolf_Server;
using Werewolf_Server.GameFiles.Roles.Active;
using Werewolf_Server.GameFiles.Roles.Seer;

namespace WerewolfServerTest.Tests
{
    public class SeerTests : RoleTestFunctions
    {

        [Theory]
        [InlineData("Werewolf", true)]
        [InlineData("Lycan", true)]
        [InlineData("Villager", false)]
        [InlineData("Seer", false)]
        public void Seer_Submit(string selectedRole, bool expectedWerewolf)
        {
            InitGameForNight(4, "Custom;Seer;Werewolf;Villager;Lycan");
            Player seer = game.GetPlayerByRole("Seer");
            Player selectedPlayer = game.GetPlayerByRole(selectedRole);
            seer.Should().NotBeNull();

            SetServerMessage(seer.name, selectedPlayer.name);
            var result = game.Update(serverMessage);

            if (!GetNightMessage(result)) { return; }
            if (expectedWerewolf)
            {
                nightInfoMessage.data[0].Should().Contain("IS");
            }
            else
            {
                nightInfoMessage.data[0].Should().Contain("NOT");
            }
        }

        [Fact]
        public void ApprenticeSeer_Submit()
        {
            InitGameForNight(4, "Custom;Seer;Apprentice Seer;Werewolf;Villager");
            Player seer = game.GetPlayerByRole("Seer");
            Player apprentice = game.GetPlayerByRole("Apprentice Seer");
            Player werewolf = game.GetPlayerByRole("Werewolf");


            //Submit while Seer is still alive
            SetServerMessage(apprentice.name, "0");

            var result = game.Update(serverMessage);

            //Kill the Seer
            //Ready up everyone to finish the night
            foreach (Player player in game.AlivePlayers)
            {
                player.ready = true;
            }
            seer.werewolvesAttacking++;
            SetServerMessage(werewolf.name, seer.name);
            game.Update(serverMessage);

            //Submit while Seer is still dead
            SetServerMessage(apprentice.name, werewolf.name);
            result = game.Update(serverMessage);
            if (!GetNightMessage(result)) { return; }
            nightInfoMessage.data[0].Should().Contain("Werewolf");

        }


        [Theory]
        [InlineData("Werewolf", true)]
        [InlineData("Villager", false)]
        [InlineData("Seer", false)]
        public void Revealer_Submit(string findRole, bool theyDie)
        {
            InitGameForNight(4, "Custom;Revealer;Werewolf;Villager;Seer");
            Player revealer = game.GetPlayerByRole("Revealer");
            Player selectedPlayer = game.GetPlayerByRole(findRole);
            SetServerMessage(revealer.name, selectedPlayer.name);


            game.Update(serverMessage);
            game.FinishNight();

            if (theyDie)
            {
                revealer.alive.Should().BeTrue();
                selectedPlayer.alive.Should().BeFalse();
            }
            else
            {
                revealer.alive.Should().BeFalse();
                selectedPlayer.alive.Should().BeTrue();
            }
        }

        [Fact]
        public void MysticSeer_Submit()
        {
            Role testRole = new ApprenticeSeer();
            //Test with the apprentice seer i guess
            InitGameForNight(2, "Custom;Mystic Seer;Apprentice Seer");
            Player mysticSeer = game.GetPlayerByRole("Mystic Seer");
            Player aprenticeSeer = game.GetPlayerByRole("Apprentice Seer");

            SetServerMessage(mysticSeer.name, aprenticeSeer.name);
            var result = game.Update(serverMessage);
            if (!GetNightMessage(result)) { return; }

            //First message is the name
            nightInfoMessage.data[0].Should().Contain(testRole.name);
            //Second message is the description
            nightInfoMessage.data[1].Should().Contain(testRole.description);

        }

        [Theory]
        [InlineData("Villager", "Seer", true)]
        [InlineData("Villager", "Lycan", false)]
        [InlineData("Villager", "Werewolf", false)]
        [InlineData("Werewolf", "Werewolf", true)]
        public void Mentalist_Submit(string team1, string team2, bool expectedSameTeam)
        {
            string players = $"Custom;Mentalist;{team1};{team2}";
            InitGameForNight(3, players);
            Player mentalist = game.GetPlayerByRole("Mentalist");
            Player player1 = game.GetPlayerByRole(team1);
            Player player2 = game.GetPlayerByRole(team2);

            SetServerMessage(mentalist.name, "");
            serverMessage.data = new List<string> { player1.name, player2.name };

            var result = game.Update(serverMessage);
            if (!GetNightMessage(result)) { return; }

            if (expectedSameTeam)
            {
                nightInfoMessage.data[0].Should().Contain("ARE");
            }
            else
            {
                nightInfoMessage.data[0].Should().Contain("NOT");
            }

        }

        [Theory]
        [InlineData("Seer", true)]
        [InlineData("Lycan", false)]
        [InlineData("Werewolf", false)]
        [InlineData("Villager", false)]
        public void AuraSeer_Submit(string role, bool hasAbility)
        {
            string players = $"Custom;Aura Seer;{role}";
            InitGameForNight(3, players);
            Player auraSeer = game.GetPlayerByRole("Aura Seer");
            Player selectedPlayer = game.GetPlayerByRole(role);

            SetServerMessage(auraSeer.name, selectedPlayer.name);

            var result = game.Update(serverMessage);
            if (!GetNightMessage(result)) { return; }

            if (hasAbility)
            {
                nightInfoMessage.data[0].Should().Contain("HAS");
            }
            else
            {
                nightInfoMessage.data[0].Should().Contain("NO");
            }

        }


    }
}
