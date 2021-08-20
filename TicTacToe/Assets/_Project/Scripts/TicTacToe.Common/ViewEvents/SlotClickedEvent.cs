using UnityEngine;

namespace TicTacToe.Common.ViewEvents
{
    public struct SlotClickedEvent : IViewEvent
    {
        public Vector2Int Slot;
    }
}
