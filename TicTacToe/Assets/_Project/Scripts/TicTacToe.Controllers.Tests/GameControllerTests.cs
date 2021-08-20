using NUnit.Framework;
using TicTacToe.Common;
using TicTacToe.Common.ControllerEvents;
using TicTacToe.Common.ViewEvents;
using TicTacToe.Controllers;
using UnityEngine;
using UnityEngine.TestTools;

namespace TicTacToe.Controllers.Tests
{
    [TestOf(typeof(GameController))]
    public class GameControllerTests : ControllerTestFixtureBase
    {
        private const int AIIndex = 2;

        [Test]
        public void Human_VS_Human()
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

        [Test]
        public void AI_VS_AI()
        {
            GameController gameController = new GameController(Model, EventService);

            EventService.Invoke(this, new UpdatePlayerEvent
            {
                PlayerIndex = 1,
                AIIndex = AIIndex,
                Sign = "O",
            });

            EventService.Invoke(this, new UpdatePlayerEvent
            {
                PlayerIndex = 0,
                AIIndex = AIIndex,
                Sign = "X",
            });

            bool stop = false;

            EventService.AddListener<GameEndedEvent>(delegate
            {
                stop = true;
                Debug.Log(nameof(GameEndedEvent));
                Assert.That(Model.Sequences.Count, Is.EqualTo(0));
            });

            LogAssert.Expect(LogType.Log, nameof(GameEndedEvent));

            float deltaTime = Model.AIList[AIIndex].PlayDelay / 2f;

            while (!stop)
            {
                gameController.OnUpdate(deltaTime);
            }
        }
    }
}
