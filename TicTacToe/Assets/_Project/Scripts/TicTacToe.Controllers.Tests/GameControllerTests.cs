using NUnit.Framework;
using TicTacToe.Common;
using TicTacToe.Common.ControllerEvents;
using TicTacToe.Common.ViewEvents;
using UnityEngine;
using UnityEngine.TestTools;

namespace TicTacToe.Controllers.Tests
{
    [TestOf(typeof(GameController))]
    public class GameControllerTests : ControllerTestFixtureBase
    {
        private static readonly object[] SlotValueSource =
        {
            Vector2Int.one,
            Vector2Int.zero,
            Vector2Int.one * 2,
            Vector2Int.right
        };

        [Test]
        public void AI_VS_AI()
        {
            GameController gameController = new GameController(Model, EventService);
            Model.AIList[0].Depth = 8;
            Model.AIList[0].PlayDelay = 0.1f;

            EventService.Invoke(this, new UpdatePlayerEvent
            {
                PlayerIndex = 1,
                AIIndex = 0,
                Sign = "O",
            });

            EventService.Invoke(this, new UpdatePlayerEvent
            {
                PlayerIndex = 0,
                AIIndex = 0,
                Sign = "X",
            });

            bool stop = false;

            EventService.AddListener<GameEndedEvent>(delegate
            {
                stop = true;
                Debug.Log(nameof(GameEndedEvent));
                Assert.That(Model.Board.Sequences.Count, Is.EqualTo(0));
            });

            LogAssert.Expect(LogType.Log, nameof(GameEndedEvent));

            float deltaTime = Model.AIList[0].PlayDelay / 3f;

            while (!stop)
            {
                gameController.OnUpdate(deltaTime);
            }
        }

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
                Assert.That(Model.Board.Sequences[0], Is.EqualTo(new Sequence
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
        public void SlotClicked_IgnoredWhenHasSequences([ValueSource(nameof(SlotValueSource))] Vector2Int slot)
        {
            Model.Board.Sequences = new Sequence[1];

            // ReSharper disable once UnusedVariable
            GameController gameController = new GameController(Model, EventService);
            EventService.AddListener<SlotValueChangedEvent>(delegate
            {
                Debug.Log(nameof(SlotValueChangedEvent));
            });

            EventService.Invoke(this, new SlotClickedEvent());
            LogAssert.NoUnexpectedReceived();
        }
    }
}
