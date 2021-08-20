namespace TicTacToe.Common.ControllerEvents
{
    public struct PlayerUpdatedEvent : IControllerEvent
    {
        public int PlayerIndex;
    }
}
