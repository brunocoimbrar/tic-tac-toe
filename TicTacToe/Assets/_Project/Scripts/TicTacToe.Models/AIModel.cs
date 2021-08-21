using JetBrains.Annotations;
using System;
using TicTacToe.Common;
using UnityEngine;

namespace TicTacToe.Models
{
    [Serializable]
    public sealed class AIModel : IAIModel
    {
        [SerializeField]
        private string _description = "Random";

        [SerializeField]
        [Min(0)]
        private int _depth;

        [SerializeField]
        [Min(0)]
        private float _playDelay = 0.5f;

        [NotNull]
        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public int Depth
        {
            get => _depth;
            set => _depth = Mathf.Max(value, 0);
        }

        public float PlayDelay
        {
            get => _playDelay;
            set => _playDelay = Mathf.Abs(value);
        }
    }
}
