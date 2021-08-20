using TicTacToe.Common;
using TicTacToe.Common.ControllerEvents;
using TicTacToe.Common.ViewEvents;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TicTacToe.Views
{
    public sealed class SlotView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private TextMeshProUGUI _slotText;

        private IGameModel _model;
        private IViewEventService _eventService;
        private Vector2Int _slot;

        public void Initialize(IGameModel model, IViewEventService eventService, Vector2Int slot)
        {
            _model = model;
            _eventService = eventService;
            _slot = slot;
            _eventService.AddListener<SlotValueChangedEvent>(HandleSlotValueChanged);
            UpdateSlotText();
        }

        public void Dispose()
        {
            _eventService.RemoveListener<SlotValueChangedEvent>(HandleSlotValueChanged);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _eventService.Invoke(this, new SlotClickedEvent
            {
                Slot = _slot
            });
        }

        private void HandleSlotValueChanged(object sender, SlotValueChangedEvent e)
        {
            if (e.Slot == _slot)
            {
                UpdateSlotText();
            }
        }

        private void UpdateSlotText()
        {
            int? value = _model.GetSlotValue(_slot.x, _slot.y);

            if (value.HasValue)
            {
                _slotText.text = _model.Players[value.Value].Sign;
                _slotText.color = ViewUtility.GetPlayerColor(value.Value);
            }
            else
            {
                _slotText.text = string.Empty;
            }
        }
    }
}
