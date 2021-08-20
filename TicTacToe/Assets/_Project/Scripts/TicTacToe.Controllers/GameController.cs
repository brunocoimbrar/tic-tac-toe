using JetBrains.Annotations;
using System;
using TicTacToe.Common;
using TicTacToe.Common.ControllerEvents;
using TicTacToe.Common.ViewEvents;
using TicTacToe.Models;

namespace TicTacToe.Controllers
{
    public sealed class GameController
    {
        private readonly IControllerEventService _eventService;
        private readonly GameModel _model;
        private readonly BoardController _boardController;
        private readonly AITurnController _aiTurnController;

        public GameController([NotNull] GameModel model, [NotNull] IControllerEventService eventService)
        {
            _eventService = eventService;
            _model = model;
            _boardController = new BoardController(_model, eventService);
            _aiTurnController = new AITurnController(_model, _boardController);
            _eventService.AddListener<RestartGameEvent>(HandleRestartGameEvent);
            _eventService.AddListener<SlotClickedEvent>(HandleSlotClicked);
            _eventService.AddListener<UpdatePlayerEvent>(HandleUpdatePlayer);
            _eventService.AddListener<TurnChangedEvent>(HandleTurnChanged);
        }

        public void OnUpdate(float deltaTime)
        {
            _aiTurnController.OnUpdate(deltaTime);
        }

        private void HandleSlotClicked(object sender, SlotClickedEvent e)
        {
            if (_model.Sequences.Count > 0)
            {
                return;
            }

            if (_model.Players[_model.PlayerIndex].AIIndex < 0)
            {
                _boardController.Play(e.Slot);
            }
        }

        private void HandleRestartGameEvent(object sender, RestartGameEvent e)
        {
            Restart();
        }

        private void HandleTurnChanged(object sender, TurnChangedEvent e)
        {
            int playerIndex = _model.PlayerIndex;

            if (_model.Players[_model.PlayerIndex].AIIndex >= 0)
            {
                _aiTurnController.Play(playerIndex);
            }
        }

        private void HandleUpdatePlayer(object sender, UpdatePlayerEvent e)
        {
            PlayerModel player = _model.GetPlayer(e.PlayerIndex);
            player.AIIndex = e.AIIndex == _model.AIList.Count ? -1 : e.AIIndex;
            player.Sign = e.Sign;
            _eventService.Invoke(this, new PlayerUpdatedEvent
            {
                PlayerIndex = e.PlayerIndex,
            });

            if (_model.Turn == 0 && e.PlayerIndex == 0 && player.AIIndex >= 0)
            {
                _aiTurnController.Play(0);
            }
            else
            {
                Restart();
            }
        }

        private void Restart()
        {
            _model.AITurn.RemainingPlayDelay = 0;
            _model.Turn = 0;
            _model.Sequences = Array.Empty<Sequence>();

            for (int x = 0; x < _model.BoardSize; x++)
            {
                for (int y = 0; y < _model.BoardSize; y++)
                {
                    _model.SetSlotValue(x, y, null);
                }
            }

            PlayerModel player = _model.GetPlayer(0);
            player.AIIndex = -1;
            _eventService.Invoke(this, new GameRestartedEvent());
        }
    }
}
