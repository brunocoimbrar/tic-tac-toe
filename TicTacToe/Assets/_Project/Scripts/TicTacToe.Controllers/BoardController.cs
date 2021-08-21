using JetBrains.Annotations;
using System.Collections.Generic;
using TicTacToe.Common;
using TicTacToe.Common.ControllerEvents;
using TicTacToe.Models;
using UnityEngine;

namespace TicTacToe.Controllers
{
    public sealed class BoardController
    {
        private readonly IControllerEventService _eventService;
        private readonly GameModel _model;

        public BoardController([NotNull] GameModel model, [NotNull] IControllerEventService eventService)
        {
            _model = model;
            _eventService = eventService;
        }

        public bool HasAnySequence(IReadOnlyTable<int?> board, Vector2Int slot, int playerIndex, List<Sequence> sequences = null)
        {
            bool result = false;

            if (HasBackSlashSequence(board, slot, playerIndex, out Sequence sequence))
            {
                result = true;
                sequences?.Add(sequence);
            }

            if (HasForwardSlashSequence(board, slot, playerIndex, out sequence))
            {
                result = true;
                sequences?.Add(sequence);
            }

            if (HasHorizontalSequence(board, slot, playerIndex, out sequence))
            {
                result = true;
                sequences?.Add(sequence);
            }

            if (HasVerticalSequence(board, slot, playerIndex, out sequence))
            {
                result = true;
                sequences?.Add(sequence);
            }

            return result;
        }

        public void Play(Vector2Int slot)
        {
            if (_model.Board[slot.x, slot.y].HasValue)
            {
                return;
            }

            _model.Board[slot.x, slot.y] = _model.PlayerIndex;
            _eventService.Invoke(this, new SlotValueChangedEvent
            {
                Slot = slot
            });

            int boardArea = _model.Board.Width * _model.Board.Width;
            List<Sequence> sequences = new List<Sequence>();

            if (HasAnySequence(_model.Board, slot, _model.PlayerIndex, sequences))
            {
                _model.Board.Sequences = sequences;
                _model.Players[_model.PlayerIndex].Score++;
                _eventService.Invoke(this, new GameEndedEvent());
            }
            else if (_model.Turn + 1 < boardArea)
            {
                _model.Turn++;
                _eventService.Invoke(this, new TurnChangedEvent());
            }
            else
            {
                _eventService.Invoke(this, new GameEndedEvent());
            }
        }

        private bool HasBackSlashSequence(IReadOnlyTable<int?> board, Vector2Int slot, int playerIndex, out Sequence sequence)
        {
            sequence = new Sequence();

            int sequenceSize = 1;
            sequence.From = slot;

            for (Vector2Int copy = slot - Vector2Int.one; copy.x >= 0 && copy.y >= 0; copy -= Vector2Int.one)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                sequence.From = copy;
                sequenceSize++;
            }

            sequence.To = slot;

            for (Vector2Int copy = slot + Vector2Int.one; copy.x < _model.Board.Width && copy.y < _model.Board.Width; copy += Vector2Int.one)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                sequence.To = copy;
                sequenceSize++;
            }

            return sequenceSize >= _model.Board.SequenceSize;
        }

        private bool HasForwardSlashSequence(IReadOnlyTable<int?> board, Vector2Int slot, int playerIndex, out Sequence sequence)
        {
            sequence = new Sequence();

            int sequenceSize = 1;
            sequence.From = slot;

            {
                Vector2Int increment = Vector2Int.right - Vector2Int.up;

                for (Vector2Int copy = slot + increment; copy.x < _model.Board.Width && copy.y >= 0; copy += increment)
                {
                    if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                    {
                        break;
                    }

                    sequence.From = copy;
                    sequenceSize++;
                }
            }

            sequence.To = slot;

            {
                Vector2Int increment = Vector2Int.left - Vector2Int.down;

                for (Vector2Int copy = slot + increment; copy.x >= 0 && copy.y < _model.Board.Width; copy += increment)
                {
                    if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                    {
                        break;
                    }

                    sequence.To = copy;
                    sequenceSize++;
                }
            }

            return sequenceSize >= _model.Board.SequenceSize;
        }

        private bool HasHorizontalSequence(IReadOnlyTable<int?> board, Vector2Int slot, int playerIndex, out Sequence sequence)
        {
            sequence = new Sequence();

            int sequenceSize = 1;
            sequence.From = slot;

            for (Vector2Int copy = slot + Vector2Int.left; copy.x >= 0; copy.x--)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                sequence.From = copy;
                sequenceSize++;
            }

            sequence.To = slot;

            for (Vector2Int copy = slot + Vector2Int.right; copy.x < _model.Board.Width; copy.x++)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                sequence.To = copy;
                sequenceSize++;
            }

            return sequenceSize >= _model.Board.SequenceSize;
        }

        private bool HasVerticalSequence(IReadOnlyTable<int?> board, Vector2Int slot, int playerIndex, out Sequence sequence)
        {
            sequence = new Sequence();

            int sequenceSize = 1;
            sequence.From = slot;

            for (Vector2Int copy = slot - Vector2Int.up; copy.y >= 0; copy.y--)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                sequence.From = copy;
                sequenceSize++;
            }

            sequence.To = slot;

            for (Vector2Int copy = slot - Vector2Int.down; copy.y < _model.Board.Width; copy.y++)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                sequence.To = copy;
                sequenceSize++;
            }

            return sequenceSize >= _model.Board.SequenceSize;
        }
    }
}
