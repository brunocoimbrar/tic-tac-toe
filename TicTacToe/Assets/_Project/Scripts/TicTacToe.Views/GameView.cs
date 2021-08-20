using TicTacToe.Common;
using TicTacToe.Common.ControllerEvents;
using TicTacToe.Common.ViewEvents;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe.Views
{
    public sealed class GameView : MonoBehaviour
    {
        [SerializeField]
        private BoardView _boardView;

        [SerializeField]
        private TurnView _turnView;

        [SerializeField]
        private GameObject _gameOverIndicator;

        [SerializeField]
        private RectTransform _playerViewContainer;

        [SerializeField]
        private PlayerView _playerViewPrefab;

        [SerializeField]
        private Button _restartGameButton;

        private IGameModel _model;
        private IViewEventService _eventService;
        private PlayerView[] _playerViews;

        public void Initialize(IGameModel model, IViewEventService eventService)
        {
            _model = model;
            _eventService = eventService;
            _eventService.AddListener<GameRestartedEvent>(HandleGameRestarted);
            _eventService.AddListener<GameEndedEvent>(HandleGameEnded);
            _restartGameButton.onClick.AddListener(HandleRestartGameButtonClick);
            CreatePlayerViews();
            Initialize();
        }

        private void CreatePlayerViews()
        {
            _playerViews = new PlayerView[_model.Players.Count];

            for (int i = 0; i < _playerViews.Length; i++)
            {
                _playerViews[i] = Instantiate(_playerViewPrefab, _playerViewContainer, false);
            }
        }

        private void HandleGameEnded(object sender, GameEndedEvent e)
        {
            _turnView.Dispose();
            _gameOverIndicator.SetActive(true);
        }

        private void HandleGameRestarted(object sender, GameRestartedEvent e)
        {
            _boardView.Dispose();
            _turnView.Dispose();

            foreach (PlayerView playerView in _playerViews)
            {
                playerView.Dispose();
            }

            Initialize();
        }

        private void HandleRestartGameButtonClick()
        {
            _eventService.Invoke(this, new RestartGameEvent());
        }

        private void Initialize()
        {
            _gameOverIndicator.SetActive(false);
            _boardView.Initialize(_model, _eventService);
            _turnView.Initialize(_model, _eventService);
            InitializePlayerViews();
        }

        private void InitializePlayerViews()
        {
            for (int i = 0; i < _playerViews.Length; i++)
            {
                _playerViews[i].Initialize(_model, _eventService, i);
            }
        }
    }
}
