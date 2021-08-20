using System;
using UnityEngine;

namespace TicTacToe.Models
{
    [Serializable]
    public sealed class AITurnModel
    {
        public float RemainingPlayDelay { get; set; }

        public Vector2Int Slot { get; set; }
    }
}
