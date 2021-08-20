using TicTacToe.Common;
using TicTacToe.Common.ControllerEvents;
using TMPro;
using UnityEngine;

namespace TicTacToe.Views
{
    public sealed class TurnView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _turnText;

        private IGameModel _model;
        private IViewEventService _eventService;

        public void Initialize(IGameModel model, IViewEventService eventService)
        {
            _model = model;
            _eventService = eventService;
            _eventService.AddListener<TurnChangedEvent>(HandleTurnChanged);
            gameObject.SetActive(true);
            UpdateTurnText();
        }

        public void Dispose()
        {
            gameObject.SetActive(false);
            _eventService.RemoveListener<TurnChangedEvent>(HandleTurnChanged);
        }

        private void HandleTurnChanged(object sender, TurnChangedEvent e)
        {
            UpdateTurnText();
        }

        private void UpdateTurnText()
        {
            int playerIndex = _model.PlayerIndex;
            _turnText.text = _model.Players[playerIndex].Sign;
            _turnText.color = ViewUtility.GetPlayerColor(playerIndex);
        }
    }
}
