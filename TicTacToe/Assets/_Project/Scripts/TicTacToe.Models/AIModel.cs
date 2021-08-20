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
        [Range(0, 4)]
        private int _depth;

        [SerializeField]
        [Range(0, 1)]
        private float _playDelay = 0.5f;

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public int Depth
        {
            get => _depth;
            set => _depth = value;
        }

        public float PlayDelay
        {
            get => _playDelay;
            set => _playDelay = value;
        }
    }
}
