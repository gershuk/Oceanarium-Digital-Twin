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

        private bool _isInited = false;

        [SerializeField]
        private PlayerInventory _model;

        [SerializeField]
        private GameObject _selectItemViewPrefab;

        public PlayerInventory Model
        {
            get => _model;
            set
            {
                _model = value;
                _model.ForceInit();
                SubcribeToModel();
            }
        }

        private void AddFrame (IInfo info)
        {
            var selectItemViewModel = Instantiate(_selectItemViewPrefab, _content).GetComponent<SelectItemViewModel>();
            selectItemViewModel.ForceInit();
            selectItemViewModel.Model = info;
            _items.Add(selectItemViewModel);
        }

        private void Awake () => ForceInit();

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
            _indexSocket.SubscribeTo(Model.SelectedItemSocket);
            Model.Inventory.ObserveAdd().Subscribe(e => AddFrame(e.Value)).AddTo(_collectionDisposables);
            Model.Inventory.ObserveRemove().Subscribe(e => RemoveFrame(e.Value)).AddTo(_collectionDisposables);
            Model.Inventory.ObserveCountChanged().Subscribe(UpdatePanelVisual).AddTo(_collectionDisposables);
        }

        private void UnsubcribeFromModel ()
        {
            _indexSocket.UnsubscribeFrom(Model.SelectedItemSocket);
            _collectionDisposables.Dispose();
        }

        private void UpdatePanelVisual (int count) => _content.GetComponent<Image>().enabled = count > 0;

        public void ForceInit ()
        {
            if (_isInited)
                return;

            if (_content == null)
                _content = GetComponent<RectTransform>();

            _indexSocket.ReadOnlyProperty.Subscribe(SetFrameSelected).AddTo(this);

            UpdatePanelVisual(0);
            _isInited = true;
        }
    }
}