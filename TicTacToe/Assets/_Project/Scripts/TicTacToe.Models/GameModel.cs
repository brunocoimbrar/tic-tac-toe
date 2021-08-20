using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TicTacToe.Common;
using UnityEngine;

namespace TicTacToe.Models
{
    [Serializable]
    public sealed class GameModel : IGameModel, ISerializationCallbackReceiver
    {
        [SerializeField]
        [Min(MinBoardSize)]
        private int _boardSize = MinBoardSize;

        [SerializeField]
        [Min(MinBoardSize)]
        private int _sequenceSize = MinBoardSize;

        [SerializeField]
        private List<PlayerModel> _players = new List<PlayerModel>
        {
            new PlayerModel
            {
                Sign = "X",
                AIIndex = -1,
            },
            new PlayerModel
            {
                Sign = "O",
                AIIndex = 1
            }
        };

        [SerializeField]
        private List<AIModel> _aiList = new List<AIModel>
        {
            new AIModel
            {
                Description = "Easy",
                Depth = 1,
            },
            new AIModel
            {
                Description = "Medium",
                Depth = 2,
            },
            new AIModel
            {
                Description = "Hard",
                Depth = 4,
            },
        };

        private const int MinBoardSize = 3;
        private int?[,] _board = new int?[MinBoardSize, MinBoardSize];

        public AITurnModel AITurn { get; } = new AITurnModel();

        public int BoardSize => _boardSize;

        public int PlayerIndex => Turn % _players.Count;

        public int SequenceSize => _sequenceSize;

        public int Turn { get; set; }

        [NotNull]
        [ItemNotNull]
        public IReadOnlyList<IAIModel> AIList => _aiList;

        [NotNull]
        [ItemNotNull]
        public IReadOnlyList<IPlayerModel> Players => _players;

        [NotNull]
        public IReadOnlyList<Sequence> Sequences { get; set; } = Array.Empty<Sequence>();

        [NotNull]
        public PlayerModel GetPlayer(int playerIndex)
        {
            return _players[playerIndex];
        }

        public int? GetSlotValue(int x, int y)
        {
            return _board[x, y];
        }

        public void SetSlotValue(int x, int y, int? value)
        {
            _board[x, y] = value;
        }

        private void SetBoardSize(int value)
        {
            if (value < MinBoardSize)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be at least {MinBoardSize}!");
            }

            if (_board.Length == value)
            {
                return;
            }

            _boardSize = value;
            _board = new int?[_boardSize, _boardSize];
            _sequenceSize = Mathf.Clamp(_sequenceSize, MinBoardSize, _boardSize);
        }

        private void SetPlayerCount(int value)
        {
            if (value < MinBoardSize - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be between {MinBoardSize - 1} and {BoardSize - 1}!");
            }

            if (_players.Count == value)
            {
                return;
            }

            if (_players.Count > value)
            {
                _players.RemoveRange(value, _players.Count - value);
            }
            else
            {
                do
                {
                    _players.Add(new PlayerModel());
                }
                while (_players.Count < value);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _players[0].AIIndex = -1;

            for (int i = 1; i < _players.Count; i++)
            {
                _players[i].AIIndex = Mathf.Clamp(_players[i].AIIndex, -1, _aiList.Count - 1);
            }

            SetBoardSize(_boardSize);

            try
            {
                SetPlayerCount(_players.Count);
            }
            catch
            {
                SetPlayerCount(BoardSize - 1);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    }
}
