using JetBrains.Annotations;
using System;
using TicTacToe.Common;
using UnityEngine;

namespace TicTacToe.Models
{
    [Serializable]
    public sealed class PlayerModel : IPlayerModel
    {
        [SerializeField]
        private string _sign = "X";

        [SerializeField]
        [Min(-1)]
        private int _aiIndex = -1;

        [NotNull]
        public string Sign
        {
            get => _sign;
            set => _sign = value;
        }

        public int AIIndex
        {
            get => _aiIndex;
            set => _aiIndex = Mathf.Max(value, -1);
        }

        public int Score { get; set; }
    }
}
