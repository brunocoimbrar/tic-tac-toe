using JetBrains.Annotations;
using TicTacToe.Common;
using TicTacToe.Controllers;
using TicTacToe.Views;
using UnityEngine;

namespace TicTacToe
{
    public sealed class GameSystem : MonoBehaviour
    {
        [SerializeField]
        private GameSystemData _data;

        [SerializeField]
        private GameView _view;

        private GameController _controller;
        private EventService _eventService;
        private GameSystemData _runtimeData;

        private void Awake()
        {
            _eventService = new EventService();
            _runtimeData = Instantiate(_data);
        }

        private void Start()
        {
            _controller = new GameController(_runtimeData.Model, _eventService);
            _view.Initialize(_runtimeData.Model, _eventService);
        }

        private void Update()
        {
            _controller.OnUpdate(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _controller = null;
            DestroyImmediate(_runtimeData);
        }
    }
}
