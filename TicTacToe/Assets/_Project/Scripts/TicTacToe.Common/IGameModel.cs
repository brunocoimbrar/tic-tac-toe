using System.Collections.Generic;

namespace TicTacToe.Common
{
    public interface IGameModel
    {
        int BoardSize { get; }

        int PlayerIndex { get; }

        int SequenceSize { get; }

        int Turn { get; }

        IReadOnlyList<IAIModel> AIList { get; }

        IReadOnlyList<IPlayerModel> Players { get; }

        IReadOnlyList<Sequence> Sequences { get; }

        int? GetSlotValue(int x, int y);
    }
}
