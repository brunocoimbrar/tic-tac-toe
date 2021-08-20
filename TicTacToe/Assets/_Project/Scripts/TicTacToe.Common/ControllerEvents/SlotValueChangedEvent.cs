using UnityEngine;

namespace TicTacToe.Common.ControllerEvents
{
    public struct SlotValueChangedEvent : IControllerEvent
    {
        public Vector2Int Slot;
    }
}
