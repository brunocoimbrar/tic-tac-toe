using System.Collections.Generic;
using TicTacToe.Common;
using TicTacToe.Models;
using UnityEngine;

namespace TicTacToe.Controllers
{
    public sealed class AITurnController
    {
        private readonly GameModel _model;
        private readonly BoardController _boardController;

        public AITurnController(GameModel model, BoardController boardController)
        {
            _model = model;
            _boardController = boardController;
        }

        public void OnUpdate(float deltaTime)
        {
            AITurnModel aiTurn = _model.AITurn;

            if (aiTurn.RemainingPlayDelay <= 0)
            {
                return;
            }

            aiTurn.RemainingPlayDelay -= deltaTime;

            if (aiTurn.RemainingPlayDelay <= 0)
            {
                _boardController.Play(aiTurn.Slot);
            }
        }

        public void Play(int playerIndex)
        {
            IPlayerModel player = _model.Players[playerIndex];
            IAIModel ai = _model.AIList[player.AIIndex];
            AITurnModel aiTurn = _model.AITurn;
            aiTurn.Slot = Play(playerIndex, ai.Depth);

            if (ai.PlayDelay > 0)
            {
                aiTurn.RemainingPlayDelay = ai.PlayDelay;
            }
            else
            {
                _boardController.Play(aiTurn.Slot);
            }
        }

        private IReadOnlyList<Vector2Int> GetAvailableSlots()
        {
            List<Vector2Int> availableSlots = new List<Vector2Int>();

            for (int x = 0; x < _model.BoardSize; x++)
            {
                for (int y = 0; y < _model.BoardSize; y++)
                {
                    if (_model.GetSlotValue(x, y).HasValue)
                    {
                        continue;
                    }

                    availableSlots.Add(new Vector2Int(x, y));
                }
            }

            return availableSlots;
        }

        private Vector2Int Play(int playerIndex, int targetDepth)
        {
            int? loseMoveDepth = null;
            int? winMoveDepth = null;
            int playerTurn = _model.Turn;
            int playerCount = _model.Players.Count;
            List<Vector2Int> choices = new List<Vector2Int>();
            IReadOnlyList<Vector2Int> availableSlots = GetAvailableSlots();
            int?[,] board = null;
            _boardController.CopyBoard(ref board);

            for (int depth = 0; depth < targetDepth && loseMoveDepth == null; depth++)
            {
                if (availableSlots.Count - targetDepth == 1)
                {
                    break;
                }

                foreach (Vector2Int availableSlot in availableSlots)
                {
                    int playerTurnCopy = playerTurn;

                    if (!_boardController.TrySimulatePlay(board, availableSlot, playerIndex, ref playerTurn, out IReadOnlyList<Sequence> sequences))
                    {
                        continue;
                    }

                    if (sequences != null && sequences.Count > 0)
                    {
                        if (winMoveDepth == null || winMoveDepth > depth)
                        {
                            winMoveDepth = depth;
                            targetDepth = depth;
                            choices.Clear();
                        }

                        choices.Add(availableSlot);
                    }

                    board[availableSlot.x, availableSlot.y] = null;
                    playerTurn = playerTurnCopy;
                }

                for (int offset = 1; offset < playerCount; offset++)
                {
                    foreach (Vector2Int availableSlot in availableSlots)
                    {
                        int playerTurnCopy = playerTurn;
                        int offsetPlayerIndex = (playerIndex + offset) % _model.Players.Count;

                        if (!_boardController.TrySimulatePlay(board, availableSlot, offsetPlayerIndex, ref playerTurn, out IReadOnlyList<Sequence> sequences))
                        {
                            continue;
                        }

                        if (sequences != null && sequences.Count > 0)
                        {
                            if (loseMoveDepth == null || loseMoveDepth > depth)
                            {
                                loseMoveDepth = depth;
                                targetDepth = depth;
                                choices.Clear();
                            }

                            choices.Add(availableSlot);
                        }

                        board[availableSlot.x, availableSlot.y] = null;
                        playerTurn = playerTurnCopy;
                    }
                }
            }

            if (choices.Count == 0)
            {
                choices.AddRange(availableSlots);
            }

            return choices.Count == 1 ? choices[0] : choices[Random.Range(0, choices.Count)];
        }
    }
}
