using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TicTacToe.Common;
using UnityEngine;

namespace TicTacToe.Controllers
{
    [Serializable]
    public sealed class BoardModel : IBoardModel, ISerializationCallbackReceiver
    {
        [SerializeField]
        [Min(MinWidth)]
        private int _sequenceSize = MinWidth;

        [SerializeField]
        [Min(MinWidth)]
        private int _width = MinWidth;

        public const int MinWidth = 3;

        private int?[] _slots = new int?[MinWidth * MinWidth];

        public int? this[int x, int y]
        {
            get => _slots[_width * y + x];
            set => _slots[_width * y + x] = value;
        }

        public int SequenceSize => _sequenceSize;

        public int Width => _width;

        [NotNull]
        public IReadOnlyList<Sequence> Sequences { get; set; } = Array.Empty<Sequence>();

        private void SetBoardSize(int value)
        {
            if (value < MinWidth)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be at least {MinWidth}!");
            }

            if (_width == value && _slots.Length == value * value)
            {
                return;
            }

            _width = value;
            _slots = new int?[_width * _width];
            _sequenceSize = Mathf.Clamp(_sequenceSize, MinWidth, _width);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            SetBoardSize(_width);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    }
}
