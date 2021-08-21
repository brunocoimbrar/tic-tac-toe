using System.Collections.Generic;
using TicTacToe.Common;
using TicTacToe.Common.ControllerEvents;
using UnityEngine;

namespace TicTacToe.Views
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _columnContainer;

        [SerializeField]
        private RectTransform _columnPrefab;

        [SerializeField]
        private SlotView _slotViewPrefab;

        [SerializeField]
        private LineRenderer _lineRendererPrefab;

        private readonly Dictionary<Vector2Int, SlotView> _slotViews = new Dictionary<Vector2Int, SlotView>();
        private readonly List<GameObject> _dependencies = new List<GameObject>();
        private IGameModel _model;
        private IViewEventService _eventService;

        public void Initialize(IGameModel model, IViewEventService eventService)
        {
            _model = model;
            _eventService = eventService;

            for (int x = 0; x < model.Board.Width; x++)
            {
                RectTransform column = Instantiate(_columnPrefab, _columnContainer, false);

                for (int y = 0; y < model.Board.Width; y++)
                {
                    Vector2Int slot = new Vector2Int(x, y);
                    SlotView slotView = Instantiate(_slotViewPrefab, column, false);
                    slotView.Initialize(model, eventService, slot);
                    _slotViews.Add(slot, slotView);
                }

                _dependencies.Add(column.gameObject);
            }

            _eventService.AddListener<GameEndedEvent>(HandleGameEnded);
        }

        public void Dispose()
        {
            _eventService.RemoveListener<GameEndedEvent>(HandleGameEnded);

            foreach (SlotView slotView in _slotViews.Values)
            {
                slotView.Dispose();
            }

            _slotViews.Clear();

            foreach (GameObject column in _dependencies)
            {
                Destroy(column);
            }

            _dependencies.Clear();
        }

        private void HandleGameEnded(object sender, GameEndedEvent e)
        {
            foreach (Sequence sequence in _model.Board.Sequences)
            {
                LineRenderer lineRenderer = Instantiate(_lineRendererPrefab);
                lineRenderer.SetPosition(0, (Vector2)_slotViews[sequence.From].transform.position);
                lineRenderer.SetPosition(1, (Vector2)_slotViews[sequence.To].transform.position);
                _dependencies.Add(lineRenderer.gameObject);
            }
        }
    }
}
