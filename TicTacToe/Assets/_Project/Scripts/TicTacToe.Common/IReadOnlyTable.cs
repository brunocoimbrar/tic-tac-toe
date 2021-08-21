namespace TicTacToe.Common
{
    public interface IReadOnlyTable<out T>
    {
        int Width { get; }

        T this[int x, int y] { get; }
    }
}
