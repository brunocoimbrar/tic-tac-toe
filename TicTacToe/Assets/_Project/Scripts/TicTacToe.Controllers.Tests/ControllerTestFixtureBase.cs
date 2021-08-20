using NUnit.Framework;
using TicTacToe.Common;
using TicTacToe.Models;

namespace TicTacToe.Tests
{
    public abstract class ControllerTestFixtureBase
    {
        protected EventService EventService { get; private set; }

        protected GameModel Model { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            EventService = new EventService();
            Model = new GameModel();
        }

        [TearDown]
        public virtual void TearDown()
        {
            EventService.Dispose();
        }
    }
}
