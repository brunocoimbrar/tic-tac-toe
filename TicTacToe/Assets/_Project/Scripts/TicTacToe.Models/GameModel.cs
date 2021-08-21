using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TicTacToe.Common;
using TicTacToe.Controllers;
using UnityEngine;

namespace TicTacToe.Models
{
    [Serializable]
    public sealed class GameModel : IGameModel, ISerializationCallbackReceiver
    {
        [SerializeField]
        private BoardModel _board = new BoardModel();

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

        public AITurnModel AITurn { get; } = new AITurnModel();

        public int PlayerIndex => Turn % _players.Count;

        public int Turn { get; set; }

        [NotNull]
        public BoardModel Board => _board;

        [NotNull]
        [ItemNotNull]
        public List<AIModel> AIList => _aiList;

        [NotNull]
        [ItemNotNull]
        public List<PlayerModel> Players => _players;

        private void SetPlayerCount(int value)
        {
            if (value < BoardModel.MinWidth - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be between {BoardModel.MinWidth - 1} and {_board.Width - 1}!");
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

        [NotNull]
        IBoardModel IGameModel.Board => Board;

        [NotNull]
        [ItemNotNull]
        IReadOnlyList<IAIModel> IGameModel.AIList => AIList;

        [NotNull]
        [ItemNotNull]
        IReadOnlyList<IPlayerModel> IGameModel.Players => Players;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _players[0].AIIndex = -1;

            for (int i = 1; i < _players.Count; i++)
            {
                _players[i].AIIndex = Mathf.Clamp(_players[i].AIIndex, -1, _aiList.Count - 1);
            }

            try
            {
                SetPlayerCount(_players.Count);
            }
            catch
            {
                SetPlayerCount(_board.Width - 1);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    }
}
