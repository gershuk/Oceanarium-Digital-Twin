#nullable enable

using System;

using Aqua.Items;
using Aqua.SocketSystem;

using UniRx;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Aqua.FPSController
{
    public class PlayerInventory : MonoBehaviour
    {
        private readonly ReactiveProperty<int> _inventoryIndex = new(-1);
        private Item? _currentGrabbedItem = null;
        private bool _isInited = false;

        #region Input actions

        [Header("Input actions")]
        [SerializeField]
        private InputActionReference _dropItemAction;

        [SerializeField]
        private InputActionReference _grabReleaseItemAction;

        [SerializeField]
        private InputActionReference _invenoryIndexDelta;

        [SerializeField]
        private InputActionReference _takeItemAction;

        #endregion Input actions

        #region Collection
        private readonly ReactiveCollection<Item> _inventory = new();
        public IReadOnlyReactiveCollection<Item> Inventory => _inventory;
        #endregion Collection

        #region Sockets
        private MulticonnectionSocket<int, int> _indexSocket { get; set; }
        private MulticonnectionSocket<IInfo?, IInfo?> _selectedItemSocket { get; set; }
        public IOutputSocket<int> IndexSocket => _indexSocket;
        public IOutputSocket<IInfo?> SelectedItemSocket => _selectedItemSocket;
        #endregion Sockets

        #region Other Parameters

        [Header("Other parameters")]
        [SerializeField]
        private FPSCamera _fpsCamera;

        [SerializeField]
        [Range(1f, 9f)]
        private int _inventorySize = 9;

        [SerializeField]
        private ObjectScaner _objectScaner;

        #endregion Other Parameters

        #region Input action parameters

        [Header("Input action parameters")]
        [SerializeField]
        private float _distanceOfItemDrop = 5;

        [SerializeField]
        private LayerMask _obstacleLayerMask;

        #endregion Input action parameters

        // ToDo : Replace -1 with null
        public int InventoryIndex
        {
            get => _inventoryIndex.Value;
            set
            {
                if (value < -1 || value >= InventorySize)
                {
                    Debug.LogWarning($"Out of range item index {value}");
                    return;
                }

                _inventoryIndex.Value = value;
                if (!_selectedItemSocket.TrySetValue(SelectedItem))
                {
                    Debug.LogError("Can't set socket value when he got input connection");
                }
            }
        }

        public int InventorySize { get => _inventorySize; private set => _inventorySize = value; }
        public Item? SelectedItem => InventoryIndex > -1 ? _inventory[InventoryIndex] : null;

        private void Awake () => ForceInit();

        private bool TryDropItem ()
        {
            if (SelectedItem == null)
                return false;

            var isHitted = Physics.Raycast(_fpsCamera.Camera.transform.position,
                                _fpsCamera.Camera.transform.TransformDirection(Vector3.forward),
                                out var hit,
                                _distanceOfItemDrop,
                                _obstacleLayerMask);

            var info = isHitted ? hit.transform.gameObject.GetComponent<IInfo>() : null;

            var itemDroped = false;

            switch (isHitted, info)
            {
                case (false, _):
                    SelectedItem.transform.rotation = _fpsCamera.Camera.transform.rotation;
                    SelectedItem.transform.position = _fpsCamera.Camera.transform.TransformPoint(Vector3.forward * _distanceOfItemDrop);
                    SelectedItem.Drop();
                    itemDroped = true;
                    break;

                case (true, ItemSlot itemSlot and not null):
                    itemDroped = itemSlot.TrySetItem(SelectedItem);
                    break;

                case (true, null):
                    SelectedItem.transform.rotation = _fpsCamera.Camera.transform.rotation;
                    SelectedItem.transform.position = hit.point + hit.normal;
                    SelectedItem.Drop();
                    itemDroped = true;
                    break;
            }

            if (itemDroped)
            {
                _inventory.RemoveAt(InventoryIndex);
                InventoryIndex = InventoryIndex - 1 >= 0 ? InventoryIndex - 1 : _inventory.Count - 1;
            }

            return itemDroped;
        }

        private bool TryGrabItem ()
        {
            _currentGrabbedItem = _objectScaner.ObservedObjectSocket.GetValue() switch
            {
                Item item and not null => item,
                ItemSlot slot and not null => slot.TakeItem(),
                _ => throw new NotImplementedException(),
            };

            if (_currentGrabbedItem != null)
            {
                _currentGrabbedItem.transform.parent = _fpsCamera.Camera.transform;
                _currentGrabbedItem.Take(true);
            }

            return _currentGrabbedItem != null;
        }

        private bool TryReleaseItem ()
        {
            if (_currentGrabbedItem != null)
            {
                _currentGrabbedItem.transform.parent = null;
                _currentGrabbedItem.Drop();
                _currentGrabbedItem = null;
                return true;
            }
            return false;
        }

        private bool TryTakeItem ()
        {
            if (_inventory.Count < InventorySize)
            {
                var item = _objectScaner.ObservedObjectSocket.GetValue() switch
                {
                    Item itemObject => itemObject,
                    ItemSlot itemSlot => itemSlot.TakeItem(),
                    InfoObject => null,
                    null => null,
                    _ => throw new NotImplementedException(),
                };

                if (item != null)
                {
                    _inventory.Add(item);
                    item.Take(false);
                    InventoryIndex = _inventory.Count - 1;
                    return true;
                }
            }
            return false;
        }

        private void Update ()
        {
            if (_takeItemAction.action.WasPressedThisFrame())
                TryTakeItem();

            if (_dropItemAction.action.WasPressedThisFrame())
                TryDropItem();

            if (_grabReleaseItemAction.action.WasPressedThisFrame())
            {
                _ = _currentGrabbedItem == null
                    ? TryGrabItem()
                    : TryReleaseItem();
            }

            UpdateSelectedItem();
        }

        private void UpdateSelectedItem ()
        {
            if (_inventory.Count == 0)
                return;

            var delta = _invenoryIndexDelta.action.ReadValue<Vector2>().y;
            InventoryIndex = _invenoryIndexDelta.action.ReadValue<Vector2>().y switch
            {
                > 0.0f => (InventoryIndex + 1) % _inventory.Count,
                < 0.0f => (InventoryIndex - 1) < 0 ? _inventory.Count - 1 : InventoryIndex - 1,
                0 => InventoryIndex,
                _ => throw new NotImplementedException(),
            };
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            if (_objectScaner == null)
                _objectScaner = GetComponent<ObjectScaner>();

            if (_obstacleLayerMask.value == LayerMask.GetMask(Item.NothingLayerName))
                _obstacleLayerMask = LayerMask.GetMask(Item.DefaultLayerName, Item.ItemsLayerName, Item.ItemsSlotsLayerName);
            _indexSocket = new(_inventoryIndex);
            _selectedItemSocket = new(SelectedItem);

            _isInited = true;
        }
    }
}