namespace TicTacToe.Common
{
    public interface IAIModel
    {
        string Description { get; }

        int Depth { get; }

        float PlayDelay { get; }
    }
}
