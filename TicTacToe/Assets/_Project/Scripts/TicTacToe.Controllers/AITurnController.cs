using JetBrains.Annotations;
using System.Collections.Generic;
using TicTacToe.Common;
using TicTacToe.Models;
using UnityEngine;

namespace TicTacToe.Controllers
{
    public sealed class AITurnController
    {
        private sealed class SimulationBoard : IReadOnlyTable<int?>
        {
            private readonly int?[] _slots;

            public SimulationBoard(IReadOnlyTable<int?> source)
            {
                Width = source.Width;
                _slots = new int?[Width * Width];

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        this[x, y] = source[x, y];
                    }
                }
            }

            public int Width { get; }

            public int? this[int x, int y]
            {
                get => _slots[Width * y + x];
                set => _slots[Width * y + x] = value;
            }
        }

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
            aiTurn.Slot = GetBestChoice(playerIndex, ai.Depth);

            if (ai.PlayDelay > 0)
            {
                aiTurn.RemainingPlayDelay = ai.PlayDelay;
            }
            else
            {
                _boardController.Play(aiTurn.Slot);
            }
        }

        private static void GetAvailableSlots(IReadOnlyTable<int?> table, List<Vector2Int> results)
        {
            for (int x = 0; x < table.Width; x++)
            {
                for (int y = 0; y < table.Width; y++)
                {
                    if (table[x, y].HasValue)
                    {
                        continue;
                    }

                    results.Add(new Vector2Int(x, y));
                }
            }
        }

        private Vector2Int GetBestChoice(int playerIndex, int targetDepth)
        {
            static Vector2Int getRandomChoice(IReadOnlyList<Vector2Int> choices)
            {
                return choices.Count == 1 ? choices[0] : choices[Random.Range(0, choices.Count)];
            }

            SimulationBoard board = new SimulationBoard(_model.Board);
            List<Vector2Int> allChoices = new List<Vector2Int>();
            GetAvailableSlots(board, allChoices);

            targetDepth = Mathf.Min(allChoices.Count, targetDepth);

            if (targetDepth == 0 || allChoices.Count == board.Width * board.Width)
            {
                return getRandomChoice(allChoices);
            }

            GetScoreMap(allChoices, board, playerIndex, targetDepth, out Dictionary<Vector2Int, int> scoreMap);

            int bestScore = int.MinValue;
            List<Vector2Int> bestChoices = new List<Vector2Int>();

            foreach (Vector2Int choice in allChoices)
            {
                if (!scoreMap.TryGetValue(choice, out int score) || score < bestScore)
                {
                    continue;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestChoices.Clear();
                }

                bestChoices.Add(choice);
            }

            return getRandomChoice(bestChoices);
        }

        private int GetChoiceScoreRecursive(List<Vector2Int> allChoices, SimulationBoard board, int playerIndex, int opponentIndex, int remainingDepth, bool isPlayerTurn)
        {
            if (remainingDepth <= 0)
            {
                return 0;
            }

            if (isPlayerTurn)
            {
                int result = int.MinValue;

                foreach (Vector2Int choice in allChoices)
                {
                    if (board[choice.x, choice.y].HasValue)
                    {
                        continue;
                    }

                    if (_boardController.HasAnySequence(board, choice, playerIndex))
                    {
                        return 1;
                    }

                    board[choice.x, choice.y] = playerIndex;

                    int score = GetChoiceScoreRecursive(allChoices, board, playerIndex, opponentIndex, remainingDepth - 1, false);
                    result = Mathf.Max(result, score);
                    board[choice.x, choice.y] = null;
                }

                return result;
            }
            else
            {
                int result = int.MaxValue;

                foreach (Vector2Int choice in allChoices)
                {
                    if (board[choice.x, choice.y].HasValue)
                    {
                        continue;
                    }

                    if (_boardController.HasAnySequence(board, choice, opponentIndex))
                    {
                        return -1;
                    }

                    board[choice.x, choice.y] = opponentIndex;

                    int score = GetChoiceScoreRecursive(allChoices, board, playerIndex, opponentIndex, remainingDepth - 1, true);
                    result = Mathf.Min(result, score);
                    board[choice.x, choice.y] = null;
                }

                return result;
            }
        }

        private void GetScoreMap(List<Vector2Int> allChoices, SimulationBoard board, int playerIndex, int targetDepth, [NotNull] out Dictionary<Vector2Int, int> scoreMap)
        {
            static bool isNextMoveCritical(BoardController boardController, List<Vector2Int> allChoices, IReadOnlyTable<int?> board, int playerIndex, IDictionary<Vector2Int, int> scoreMap)
            {
                bool result = false;

                foreach (Vector2Int choice in allChoices)
                {
                    bool hasAnySequence = boardController.HasAnySequence(board, choice, playerIndex);
                    scoreMap[choice] = hasAnySequence ? 1 : int.MinValue;
                    result |= hasAnySequence;
                }

                return result;
            }

            scoreMap = new Dictionary<Vector2Int, int>();

            if (isNextMoveCritical(_boardController, allChoices, board, playerIndex, scoreMap))
            {
                return;
            }

            bool hasBestChoice = false;

            for (int offset = 1; offset < _model.Players.Count; offset++)
            {
                int otherIndex = (playerIndex + offset) % _model.Players.Count;
                hasBestChoice |= isNextMoveCritical(_boardController, allChoices, board, otherIndex, scoreMap);
            }

            if (hasBestChoice || targetDepth <= 1)
            {
                return;
            }

            int opponentIndex = (playerIndex + 1) % _model.Players.Count;

            foreach (Vector2Int choice in allChoices)
            {
                board[choice.x, choice.y] = playerIndex;
                scoreMap[choice] = GetChoiceScoreRecursive(allChoices, board, playerIndex, opponentIndex, targetDepth - 1, false);
                board[choice.x, choice.y] = null;
            }
        }
    }
}
