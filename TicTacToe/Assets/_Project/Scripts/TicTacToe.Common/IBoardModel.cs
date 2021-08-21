using System.Collections.Generic;

namespace TicTacToe.Common
{
    public interface IBoardModel : IReadOnlyTable<int?>
    {
        int SequenceSize { get; }

        IReadOnlyList<Sequence> Sequences { get; }
    }
}
