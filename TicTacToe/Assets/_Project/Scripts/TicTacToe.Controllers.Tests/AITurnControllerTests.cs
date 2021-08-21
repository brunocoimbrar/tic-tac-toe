using NUnit.Framework;
using TicTacToe.Common.ControllerEvents;
using UnityEngine;
using UnityEngine.TestTools;

namespace TicTacToe.Controllers.Tests
{
    [TestOf(typeof(AITurnController))]
    public class AITurnControllerTests : ControllerTestFixtureBase
    {
        public struct PlayTestCase
        {
            public Vector3Int[] Plays;

            public Vector3Int Expected;
        }

        private static readonly object[] PlayAvoidsDefeatTestCases =
        {
            new PlayTestCase
            {
                Plays = new[]
                {
                    new Vector3Int(0, 0, 1),
                    new Vector3Int(1, 0, 1),
                    new Vector3Int(0, 2, 0),
                    new Vector3Int(2, 1, 0)
                },
                Expected = new Vector3Int(2, 0, 0)
            },
            new PlayTestCase
            {
                Plays = new[]
                {
                    new Vector3Int(2, 0, 1),
                    new Vector3Int(0, 2, 1),
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(1, 0, 0)
                },
                Expected = new Vector3Int(1, 1, 0)
            },
            new PlayTestCase
            {
                Plays = new[]
                {
                    new Vector3Int(0, 0, 1),
                    new Vector3Int(2, 1, 0),
                    new Vector3Int(2, 2, 0)
                },
                Expected = new Vector3Int(2, 0, 1)
            }
        };
        private static readonly object[] PlayEnsuresVictoryTestCases =
        {
            new PlayTestCase
            {
                Plays = new[]
                {
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(1, 0, 0),
                    new Vector3Int(0, 2, 1),
                    new Vector3Int(1, 2, 1)
                },
                Expected = new Vector3Int(2, 0, 0)
            },
            new PlayTestCase
            {
                Plays = new[]
                {
                    new Vector3Int(2, 0, 0),
                    new Vector3Int(0, 2, 0),
                    new Vector3Int(0, 1, 1),
                    new Vector3Int(1, 2, 1)
                },
                Expected = new Vector3Int(1, 1, 0)
            },
            new PlayTestCase
            {
                Plays = new[]
                {
                    new Vector3Int(0, 0, 0),
                    new Vector3Int(0, 1, 0),
                    new Vector3Int(1, 2, 0),
                    new Vector3Int(2, 1, 1),
                    new Vector3Int(2, 2, 1)
                },
                Expected = new Vector3Int(2, 0, 1)
            }
        };

        [Test]
        public void OnUpdate_DoesNothingWhenIdle()
        {
            BoardController boardController = new BoardController(Model, EventService);
            AITurnController aiTurnController = new AITurnController(Model, boardController);

            EventService.AddListener(delegate(object sender, SlotValueChangedEvent e)
            {
                Debug.Log(nameof(SlotValueChangedEvent));
            });

            aiTurnController.OnUpdate(1);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Play_RandomAI()
        {
            Model.AIList[0].Depth = 0;
            Model.AIList[0].PlayDelay = 0;
            Model.Players[0].AIIndex = 0;

            BoardController boardController = new BoardController(Model, EventService);
            AITurnController aiTurnController = new AITurnController(Model, boardController);

            EventService.AddListener(delegate(object sender, TurnChangedEvent e)
            {
                Debug.Log(nameof(TurnChangedEvent));
                Assert.That(Model.Turn, Is.EqualTo(1));
            });

            LogAssert.Expect(LogType.Log, nameof(TurnChangedEvent));
            aiTurnController.Play(0);
        }

        [Test]
        public void Play_AvoidsDefeat([ValueSource(nameof(PlayAvoidsDefeatTestCases))] PlayTestCase testCase)
        {
            Model.AIList[0].Depth = 1;
            Model.AIList[0].PlayDelay = 0;
            Model.Players[0].AIIndex = 0;
            Model.Players[1].AIIndex = 0;
            Model.Turn = testCase.Plays.Length;

            BoardController boardController = new BoardController(Model, EventService);
            AITurnController aiTurnController = new AITurnController(Model, boardController);

            foreach (Vector3Int play in testCase.Plays)
            {
                Model.Board[play.x, play.y] = play.z;
            }

            EventService.AddListener<GameEndedEvent>(delegate
            {
                Debug.Log(nameof(GameEndedEvent));
            });

            EventService.AddListener(delegate(object sender, SlotValueChangedEvent e)
            {
                Debug.Log(nameof(SlotValueChangedEvent));
                Assert.That(e.Slot, Is.EqualTo((Vector2Int)testCase.Expected));
            });

            LogAssert.Expect(LogType.Log, nameof(SlotValueChangedEvent));
            aiTurnController.Play(testCase.Expected.z);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Play_EnsuresVictory([ValueSource(nameof(PlayEnsuresVictoryTestCases))] PlayTestCase testCase)
        {
            Model.AIList[0].Depth = 1;
            Model.AIList[0].PlayDelay = 0;
            Model.Players[0].AIIndex = 0;
            Model.Players[1].AIIndex = 0;
            Model.Turn = testCase.Plays.Length;

            BoardController boardController = new BoardController(Model, EventService);
            AITurnController aiTurnController = new AITurnController(Model, boardController);

            foreach (Vector3Int play in testCase.Plays)
            {
                Model.Board[play.x, play.y] = play.z;
            }

            EventService.AddListener<GameEndedEvent>(delegate
            {
                Debug.Log(nameof(GameEndedEvent));
                Assert.That(Model.Board[testCase.Expected.x, testCase.Expected.y], Is.EqualTo(testCase.Expected.z));
            });

            LogAssert.Expect(LogType.Log, nameof(GameEndedEvent));
            aiTurnController.Play(testCase.Expected.z);
        }
    }
}
