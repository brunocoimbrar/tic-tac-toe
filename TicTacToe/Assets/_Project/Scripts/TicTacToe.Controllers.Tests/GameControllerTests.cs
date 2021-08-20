using NUnit.Framework;
using TicTacToe.Common;
using TicTacToe.Common.ControllerEvents;
using TicTacToe.Common.ViewEvents;
using TicTacToe.Controllers;
using UnityEngine;
using UnityEngine.TestTools;

namespace TicTacToe.Tests
{
    [TestOf(typeof(GameController))]
    public class GameControllerTests : ControllerTestFixtureBase
    {
        [Test]
        public void VSHuman()
        {
            // ReSharper disable once UnusedVariable
            GameController gameController = new GameController(Model, EventService);

            EventService.Invoke(this, new UpdatePlayerEvent
            {
                PlayerIndex = 1,
                AIIndex = -1,
                Sign = "O",
            });

            EventService.AddListener<GameEndedEvent>(delegate
            {
                Debug.Log(nameof(GameEndedEvent));
                Assert.That(Model.Sequences[0], Is.EqualTo(new Sequence
                {
                    From = new Vector2Int(0, 0),
                    To = new Vector2Int(2, 2)
                }));
            });

            LogAssert.Expect(LogType.Log, nameof(GameEndedEvent));

            EventService.Invoke(this, new SlotClickedEvent
            {
                Slot = new Vector2Int(0, 0)
            });

            EventService.Invoke(this, new SlotClickedEvent
            {
                Slot = new Vector2Int(1, 0)
            });

            EventService.Invoke(this, new SlotClickedEvent
            {
                Slot = new Vector2Int(1, 1)
            });

            EventService.Invoke(this, new SlotClickedEvent
            {
                Slot = new Vector2Int(2, 0)
            });

            EventService.Invoke(this, new SlotClickedEvent
            {
                Slot = new Vector2Int(2, 2)
            });
        }
    }
}
