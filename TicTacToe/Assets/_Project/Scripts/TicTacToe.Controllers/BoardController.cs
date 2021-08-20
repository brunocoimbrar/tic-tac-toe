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

        public void CopyBoard(ref int?[,] result)
        {
            result ??= new int?[_model.BoardSize, _model.BoardSize];

            for (int x = 0; x < _model.BoardSize; x++)
            {
                for (int y = 0; y < _model.BoardSize; y++)
                {
                    result[x, y] = _model.GetSlotValue(x, y);
                }
            }
        }

        public void Play(Vector2Int slot)
        {
            int playerTurn = _model.Turn;
            int?[,] board = null;
            CopyBoard(ref board);

            if (!TrySimulatePlay(board, slot, _model.PlayerIndex, ref playerTurn, out IReadOnlyList<Sequence> sequences))
            {
                return;
            }

            _model.SetSlotValue(slot.x, slot.y, board[slot.x, slot.y]);
            _eventService.Invoke(this, new SlotValueChangedEvent
            {
                Slot = slot
            });

            if (sequences != null && sequences.Count > 0)
            {
                _model.Sequences = sequences;
                _model.GetPlayer(_model.PlayerIndex).Score++;
                _eventService.Invoke(this, new GameEndedEvent());
            }
            else if (_model.Turn < playerTurn)
            {
                _model.Turn = playerTurn;
                _eventService.Invoke(this, new TurnChangedEvent());
            }
            else
            {
                _eventService.Invoke(this, new GameEndedEvent());
            }
        }

        public bool TrySimulatePlay([NotNull] int?[,] board, Vector2Int slot, int playerIndex, ref int playerTurn, out IReadOnlyList<Sequence> sequences)
        {
            if (board[slot.x, slot.y].HasValue)
            {
                sequences = null;

                return false;
            }

            board[slot.x, slot.y] = playerIndex;
            List<Sequence> result = new List<Sequence>();
            CheckBackSlashSequence(board, slot, playerIndex, result);
            CheckForwardSlashSequence(board, slot, playerIndex, result);
            CheckHorizontalSequence(board, slot, playerIndex, result);
            CheckVerticalSequence(board, slot, playerIndex, result);

            if (result.Count == 0 && _model.Turn + 1 < _model.BoardSize * _model.BoardSize)
            {
                playerTurn++;
            }

            sequences = result;

            return true;
        }

        private void CheckBackSlashSequence([NotNull] int?[,] board, Vector2Int slot, int playerIndex, List<Sequence> sequences)
        {
            int sequenceSize = 1;
            Vector2Int from = slot;

            for (Vector2Int copy = slot - Vector2Int.one; copy.x >= 0 && copy.y >= 0; copy -= Vector2Int.one)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                from = copy;
                sequenceSize++;
            }

            Vector2Int to = slot;

            for (Vector2Int copy = slot + Vector2Int.one; copy.x < _model.BoardSize && copy.y < _model.BoardSize; copy += Vector2Int.one)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                to = copy;
                sequenceSize++;
            }

            if (sequenceSize >= _model.SequenceSize)
            {
                sequences.Add(new Sequence
                {
                    From = from,
                    To = to
                });
            }
        }

        private void CheckForwardSlashSequence([NotNull] int?[,] board, Vector2Int slot, int playerIndex, List<Sequence> sequences)
        {
            int sequenceSize = 1;
            Vector2Int from = slot;

            for (Vector2Int increment = Vector2Int.right - Vector2Int.up,
                            copy = slot + increment;
                 copy.x < _model.BoardSize && copy.y >= 0;
                 copy += increment)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                from = copy;
                sequenceSize++;
            }

            Vector2Int to = slot;

            for (Vector2Int increment = Vector2Int.left - Vector2Int.down,
                            copy = slot + increment;
                 copy.x >= 0 && copy.y < _model.BoardSize;
                 copy += increment)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                to = copy;
                sequenceSize++;
            }

            if (sequenceSize >= _model.SequenceSize)
            {
                sequences.Add(new Sequence
                {
                    From = from,
                    To = to
                });
            }
        }

        private void CheckHorizontalSequence([NotNull] int?[,] board, Vector2Int slot, int playerIndex, List<Sequence> sequences)
        {
            int sequenceSize = 1;
            Vector2Int from = slot;

            for (Vector2Int copy = slot + Vector2Int.left; copy.x >= 0; copy.x--)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                from = copy;
                sequenceSize++;
            }

            Vector2Int to = slot;

            for (Vector2Int copy = slot + Vector2Int.right; copy.x < _model.BoardSize; copy.x++)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerIndex)
                {
                    break;
                }

                to = copy;
                sequenceSize++;
            }

            if (sequenceSize >= _model.SequenceSize)
            {
                sequences.Add(new Sequence
                {
                    From = from,
                    To = to
                });
            }
        }

        private void CheckVerticalSequence([NotNull] int?[,] board, Vector2Int slot, int playerTurn, List<Sequence> sequences)
        {
            int sequenceSize = 1;
            Vector2Int from = slot;

            for (Vector2Int copy = slot - Vector2Int.up; copy.y >= 0; copy.y--)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerTurn)
                {
                    break;
                }

                from = copy;
                sequenceSize++;
            }

            Vector2Int to = slot;

            for (Vector2Int copy = slot - Vector2Int.down; copy.y < _model.BoardSize; copy.y++)
            {
                if (!board[copy.x, copy.y].HasValue || board[copy.x, copy.y] != playerTurn)
                {
                    break;
                }

                to = copy;
                sequenceSize++;
            }

            if (sequenceSize >= _model.SequenceSize)
            {
                sequences.Add(new Sequence
                {
                    From = from,
                    To = to
                });
            }
        }
    }
}
