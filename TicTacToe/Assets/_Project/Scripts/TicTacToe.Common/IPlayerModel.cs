namespace TicTacToe.Common
{
    public interface IPlayerModel
    {
        public string Sign { get; }

        public int AIIndex { get; }

        public int Score { get; }
    }
}
