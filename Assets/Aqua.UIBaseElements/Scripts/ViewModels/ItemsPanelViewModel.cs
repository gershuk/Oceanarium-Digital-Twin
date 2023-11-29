using System.Collections.Generic;
using System.Linq;

using Aqua.FPSController;
using Aqua.Items;
using Aqua.SocketSystem;

using UniRx;

using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class ItemsPanelViewModel : MonoBehaviour
    {
        private readonly CompositeDisposable _collectionDisposables = new();

        private readonly MulticonnectionSocket<IInfo, IInfo> _indexSocket = new();

        private readonly List<SelectItemViewModel> _items = new();

        [SerializeField]
        private RectTransform _content;

        [SerializeField]
        private PlayerItemController _model;

        [SerializeField]
        private GameObject _selectItemViewPrefab;

        private void AddFrame (IInfo info)
        {
            var selectItemViewModel = Instantiate(_selectItemViewPrefab, _content).GetComponent<SelectItemViewModel>();
            selectItemViewModel.ForceInit();
            selectItemViewModel.Model = info;
            _items.Add(selectItemViewModel);
        }

        private void Awake ()
        {
            if (_model == null)
                _model = FindAnyObjectByType<PlayerItemController>();

            if (_content == null)
                _content = GetComponent<RectTransform>();

            _model.ForceInit();

            _indexSocket.ReadOnlyProperty.Subscribe(SetFrameSelected).AddTo(this);

            UpdatePanelVisual(0);
            SubcribeToModel();
        }

        private void OnDestroy () => UnsubcribeFromModel();

        private void RemoveFrame (IInfo info)
        {
            foreach (var item in _items.Where(v => v.Model == info))
            {
                Destroy(item.gameObject);
            };

            _items.RemoveAll(v => v.Model == info);
        }

        private void SetFrameSelected (IInfo info)
        {
            foreach (var item in _items)
            {
                item.IsSelected = item.Model == info;
            }
        }

        private void SubcribeToModel ()
        {
            _indexSocket.SubscribeTo(_model.SelectedItemSocket);
            _model.Inventory.ObserveAdd().Subscribe(e => AddFrame(e.Value)).AddTo(_collectionDisposables);
            _model.Inventory.ObserveRemove().Subscribe(e => RemoveFrame(e.Value)).AddTo(_collectionDisposables);
            _model.Inventory.ObserveCountChanged().Subscribe(UpdatePanelVisual).AddTo(_collectionDisposables);
        }

        private void UnsubcribeFromModel ()
        {
            _indexSocket.UnsubscribeFrom(_model.SelectedItemSocket);
            _collectionDisposables.Dispose();
        }

        private void UpdatePanelVisual (int count) => _content.GetComponent<Image>().enabled = count > 0;
    }
}