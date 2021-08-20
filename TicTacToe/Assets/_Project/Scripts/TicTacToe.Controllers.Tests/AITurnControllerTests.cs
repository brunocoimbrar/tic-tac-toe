using NUnit.Framework;
using System.Collections;
using TicTacToe.Common.ControllerEvents;
using TicTacToe.Controllers;
using UnityEngine;
using UnityEngine.TestTools;

namespace TicTacToe.Tests
{
    [TestOf(typeof(AITurnController))]
    public class AITurnControllerTests : ControllerTestFixtureBase
    {
        public struct PlayTestCase
        {
            public Vector3Int[] Plays;

            public Vector3Int Expected;
        }

        private const int AIIndex = 1;

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

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            Model.GetPlayer(0).AIIndex = AIIndex;
        }

        [UnityTest]
        public IEnumerator Play_AvoidsDefeat([ValueSource(nameof(PlayAvoidsDefeatTestCases))] PlayTestCase testCase)
        {
            BoardController boardController = new BoardController(Model, EventService);
            AITurnController aiTurnController = new AITurnController(Model, boardController);

            foreach (Vector3Int play in testCase.Plays)
            {
                Model.SetSlotValue(play.x, play.y, play.z);
            }

            EventService.AddListener<GameEndedEvent>(delegate
            {
                Debug.Log("Game Ended");
            });

            EventService.AddListener(delegate(object sender, SlotValueChangedEvent e)
            {
                Debug.Log(e.Slot);
            });

            LogAssert.Expect(LogType.Log, ((Vector2Int)testCase.Expected).ToString());
            aiTurnController.Play(testCase.Expected.z);

            yield return new WaitForSeconds(Model.AIList[AIIndex].PlayDelay);

            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator Play_EnsuresVictory([ValueSource(nameof(PlayEnsuresVictoryTestCases))] PlayTestCase testCase)
        {
            BoardController boardController = new BoardController(Model, EventService);
            AITurnController aiTurnController = new AITurnController(Model, boardController);

            foreach (Vector3Int play in testCase.Plays)
            {
                Model.SetSlotValue(play.x, play.y, play.z);
            }

            EventService.AddListener<GameEndedEvent>(delegate
            {
                Debug.Log(Model.GetSlotValue(testCase.Expected.x, testCase.Expected.y));
            });

            LogAssert.Expect(LogType.Log, testCase.Expected.z.ToString());
            aiTurnController.Play(testCase.Expected.z);

            yield return new WaitForSeconds(Model.AIList[AIIndex].PlayDelay);
        }
    }
}
