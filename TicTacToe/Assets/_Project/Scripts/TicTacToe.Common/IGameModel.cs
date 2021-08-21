using System.Collections.Generic;

namespace TicTacToe.Common
{
    public interface IGameModel
    {
        int PlayerIndex { get; }

        int Turn { get; }

        IBoardModel Board { get; }

        IReadOnlyList<IAIModel> AIList { get; }

        IReadOnlyList<IPlayerModel> Players { get; }
    }
}
