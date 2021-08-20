namespace TicTacToe.Common.ViewEvents
{
    public struct UpdatePlayerEvent : IViewEvent
    {
        public int PlayerIndex;

        public int AIIndex;

        public string Sign;
    }
}
