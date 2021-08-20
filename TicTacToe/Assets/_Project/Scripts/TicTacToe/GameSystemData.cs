using TicTacToe.Models;
using UnityEngine;

namespace TicTacToe
{
    [CreateAssetMenu]
    public sealed class GameSystemData : ScriptableObject
    {
        [SerializeField]
        private GameModel _model;

        public GameModel Model => _model;
    }
}
