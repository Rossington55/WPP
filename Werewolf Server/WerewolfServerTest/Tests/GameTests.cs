using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Werewolf_Server;
using Werewolf_Server.GameFiles;

namespace WerewolfServerTest.Tests
{
    public class GameTests
    {
        private readonly Game game;
        public GameTests()
        {
            game = new Game();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(10)]
        public void Game_Start(int userCount)
        {
            List<Connection> connections = new List<Connection>();
            for (int i = 0; i < userCount; i++)
            {
                connections.Add(new Connection(i.ToString(), null));
            }

            var result =  game.Start(connections);

            result.Should().BeOfType<List<NamedMessage>>();
            result.Count.Should().Be(userCount);
        }
    }
}
