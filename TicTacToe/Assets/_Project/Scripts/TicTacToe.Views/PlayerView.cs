using TicTacToe.Common;
using TicTacToe.Common.ControllerEvents;
using TicTacToe.Common.ViewEvents;
using TMPro;
using UnityEngine;

namespace TicTacToe.Views
{
    public sealed class PlayerView : MonoBehaviour
    {
        [SerializeField]
        private string _humanOption = "Human";

        [SerializeField]
        private TextMeshProUGUI _scoreText;

        [SerializeField]
        private TMP_InputField _signInputField;

        [SerializeField]
        private TMP_Dropdown _aiDropdown;

        private int _playerIndex;
        private IGameModel _model;
        private IViewEventService _eventService;

        public void Initialize(IGameModel model, IViewEventService eventService, int playerIndex)
        {
            _playerIndex = playerIndex;
            _model = model;
            _eventService = eventService;
            _signInputField.textComponent.color = ViewUtility.GetPlayerColor(_playerIndex);
            _signInputField.onEndEdit.AddListener(HandleSignInputFieldEndEdit);
            _aiDropdown.ClearOptions();

            foreach (IAIModel aiModel in _model.AIList)
            {
                _aiDropdown.options.Add(new TMP_Dropdown.OptionData(aiModel.Description));
            }

            _aiDropdown.options.Add(new TMP_Dropdown.OptionData(_humanOption));
            _aiDropdown.RefreshShownValue();
            _aiDropdown.onValueChanged.AddListener(HandleAIDropdownValueChanged);
            _eventService.AddListener<GameEndedEvent>(HandleGameEnded);
            _eventService.AddListener<PlayerUpdatedEvent>(HandlePlayerUpdated);
            UpdateShownValues();
        }

        public void Dispose()
        {
            _eventService.RemoveListener<GameEndedEvent>(HandleGameEnded);
            _eventService.RemoveListener<PlayerUpdatedEvent>(HandlePlayerUpdated);
        }

        private void HandleAIDropdownValueChanged(int value)
        {
            _eventService.Invoke(this, new UpdatePlayerEvent
            {
                PlayerIndex = _playerIndex,
                Sign = _model.Players[_playerIndex].Sign,
                AIIndex = value
            });
        }

        private void HandleGameEnded(object sender, GameEndedEvent e)
        {
            UpdateShownValues();
        }

        private void HandlePlayerUpdated(object sender, PlayerUpdatedEvent e)
        {
            UpdateShownValues();
        }

        private void HandleSignInputFieldEndEdit(string value)
        {
            _eventService.Invoke(this, new UpdatePlayerEvent
            {
                PlayerIndex = _playerIndex,
                Sign = value,
                AIIndex = _model.Players[_playerIndex].AIIndex
            });
        }

        private void UpdateShownValues()
        {
            IPlayerModel player = _model.Players[_playerIndex];
            int ai = player.AIIndex;
            _aiDropdown.SetValueWithoutNotify(ai < 0 ? _model.AIList.Count : ai);
            _aiDropdown.RefreshShownValue();
            _signInputField.SetTextWithoutNotify(player.Sign);
            _scoreText.text = player.Score.ToString();
        }
    }
}
