using NUnit.Framework;
using TicTacToe.Common.ControllerEvents;
using UnityEngine;
using UnityEngine.TestTools;

namespace TicTacToe.Controllers.Tests
{
    [TestOf(typeof(BoardController))]
    public class BoardControllerTests : ControllerTestFixtureBase
    {
        [Test]
        public void Play_IgnoresIfInvalid()
        {
            BoardController boardController = new BoardController(Model, EventService);
            boardController.Play(Vector2Int.one);

            EventService.AddListener(delegate(object sender, SlotValueChangedEvent e)
            {
                Debug.Log(e.Slot);
            });

            boardController.Play(Vector2Int.one);
            LogAssert.NoUnexpectedReceived();
            Assert.That(Model.Board[1, 1]!.Value, Is.EqualTo(0));
        }

        [Test]
        public void Play_InvokesSlotValueChanged()
        {
            BoardController boardController = new BoardController(Model, EventService);

            EventService.AddListener(delegate(object sender, SlotValueChangedEvent e)
            {
                Debug.Log(e.Slot);
                Assert.That(Model.Board[1, 1]!.Value, Is.EqualTo(0));
            });

            LogAssert.Expect(LogType.Log, Vector2Int.one.ToString());
            boardController.Play(Vector2Int.one);
        }
    }
}
