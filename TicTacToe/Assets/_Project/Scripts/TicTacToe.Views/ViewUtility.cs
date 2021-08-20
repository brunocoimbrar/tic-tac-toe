using UnityEngine;

namespace TicTacToe.Views
{
    public static class ViewUtility
    {
        public static Color GetPlayerColor(int playerIndex)
        {
            Random.State state = Random.state;
            Random.InitState(playerIndex + 1);

            Color color = Random.ColorHSV();
            Random.state = state;

            return color;
        }
    }
}
