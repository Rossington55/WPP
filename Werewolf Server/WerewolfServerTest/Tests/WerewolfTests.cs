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
    public class WerewolfTests: RoleTests
    {

        [Fact]
        public void Werewolf_Submit()
        {
            InitGameForNight(3, "Custom;Villager;Villager;Werewolf");
            Player werewolf = game.GetPlayerByRole("Werewolf");
            werewolf.Should().NotBeNull();

            Message biteMessage = new Message(werewolf.name, CommandServer.WerewolfSelectPlayer, "0");
            biteMessage.subCommand = "select";
            SetServerMessage(werewolf.name, "");

            //Bite a playerd
            game.Update(biteMessage);


            var result = game.Update(serverMessage);

            //Submitted, update host, update all players of day, murdered
            result.Should().HaveCountGreaterThan(3);
            if (result.Count == 0) { return; }

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

        [Theory]
        [InlineData("Seer", true)]
        [InlineData("Villager", false)]
        [InlineData("Apprentice Seer", false)]
        public void Sorceress_Submit(string selectedPlayer, bool expectedSeer)
        {
            InitGameForNight(3, $"Custom;Sorceress;{selectedPlayer}");
            Player sorceress = game.GetPlayerByRole("Sorceress");
            Player labRat = game.GetPlayerByRole(selectedPlayer);

            SetServerMessage(sorceress.name, labRat.name);
            var result = game.Update(serverMessage);
            if (!GetNightMessage(result)) { return; }

            if (expectedSeer)
            {
                nightInfoMessage.data[0].Should().Contain("IS");
            }
            else
            {
                nightInfoMessage.data[0].Should().Contain("NOT");
            }
        }
    }
}
