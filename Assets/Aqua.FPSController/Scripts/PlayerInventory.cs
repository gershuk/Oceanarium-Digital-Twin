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
        private IInfo? _currentObservedObject;
        private bool _isInited = false;

        #region Layers names
        public const string DefaultLayerName = "Default";
        public const string IgnoreRaycastLayerName = "Ignore Raycast";
        public const string ItemsLayerName = "Items";
        public const string ItemsSlotsLayerName = "ItemSlots";
        public const string NothingLayerName = "Nothing";
        #endregion Layers names

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

        #endregion Other Parameters

        #region Input action parameters

        [Header("Input action parameters")]
        [SerializeField]
        private float _distanceOfItemDrop = 5;

        [SerializeField]
        private float _distanceOfItemInteraction = 5;

        [SerializeField]
        private LayerMask _obstacleLayerMask;

        #endregion Input action parameters

        public int InventoryIndex
        {
            get => _inventoryIndex.Value;
            set
            {
                if (value < 0 || value >= InventorySize)
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
        public IInfo? SelectedItem => InventoryIndex > -1 ? _inventory[InventoryIndex] : null;

        private void Awake () => ForceInit();

        private bool TryDropItem ()
        {
            if (InventoryIndex >= _inventory.Count || InventoryIndex < 0)
                return false;

            var item = _inventory[InventoryIndex];

            if (!Physics.Raycast(_fpsCamera.Camera.transform.position,
                                _fpsCamera.Camera.transform.TransformDirection(Vector3.forward),
                                out var hit,
                                _distanceOfItemDrop,
                                _obstacleLayerMask))
            {
                item.transform.rotation = _fpsCamera.Camera.transform.rotation;
                item.transform.position = _fpsCamera.Camera.transform.TransformPoint(Vector3.forward * _distanceOfItemDrop);
                item.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                switch (hit.transform.gameObject.GetComponent<IInfo>())
                {
                    case ItemSlot itemSlotScript:
                        if (itemSlotScript.CurrentItem is null)
                        {
                            itemSlotScript.SetItem(item);
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    default:
                        item.transform.rotation = _fpsCamera.Camera.transform.rotation;
                        item.transform.position = hit.point + hit.normal;
                        item.GetComponent<Rigidbody>().isKinematic = false;
                        break;
                }
            }
            _inventory.RemoveAt(InventoryIndex);
            item.gameObject.SetActive(true);

            InventoryIndex = _inventory.Count - 1;
            return true;
        }

        private bool TryGrabItem ()
        {
            if (_currentObservedObject as Item is not null)
            {
                _currentGrabbedItem = (Item) _currentObservedObject;
                _currentObservedObject = null;
            }
            if (_currentObservedObject as ItemSlot is not null)
            {
                var itemSlot = (ItemSlot) _currentObservedObject;
                _currentGrabbedItem = itemSlot.CurrentItem;
                if (_currentGrabbedItem is not null)
                    itemSlot.RemoveItem();
            }
            if (_currentGrabbedItem is not null)
            {
                _currentGrabbedItem.gameObject.layer = LayerMask.NameToLayer(IgnoreRaycastLayerName);
                _currentGrabbedItem.transform.parent = _fpsCamera.Camera.transform;
                _currentGrabbedItem.Rigidbody.isKinematic = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryReleaseItem ()
        {
            if (_currentGrabbedItem is not null)
            {
                _currentGrabbedItem.gameObject.layer = LayerMask.NameToLayer(ItemsLayerName);
                _currentGrabbedItem.transform.parent = null;
                _currentGrabbedItem.Rigidbody.isKinematic = false;
                _currentGrabbedItem = null;
                return true;
            }
            return false;
        }

        private bool TryTakeItem ()
        {
            if (_currentObservedObject is not null && _inventory.Count < InventorySize)
            {
                switch (_currentObservedObject)
                {
                    case Item itemScript:
                        itemScript.Rigidbody.isKinematic = true;
                        itemScript.gameObject.SetActive(false);
                        _inventory.Add(itemScript);
                        _currentObservedObject = null;
                        break;

                    case ItemSlot itemSlotScript:
                        var itemInSlot = itemSlotScript.CurrentItem;
                        if (itemInSlot is not null)
                        {
                            _inventory.Add(itemInSlot);
                            itemSlotScript.RemoveItem();
                            itemInSlot.gameObject.SetActive(false);
                        }
                        break;
                }
                InventoryIndex = _inventory.Count - 1;
                return true;
            }
            return false;
        }

        private void Update ()
        {
            _currentObservedObject = Physics.Raycast(_fpsCamera.Camera.transform.position,
                                                   _fpsCamera.Camera.transform.TransformDirection(Vector3.forward),
                                                   out var hit,
                                                   _distanceOfItemInteraction,
                                                   _obstacleLayerMask)
                ? hit.transform.gameObject.GetComponent<IInfo>()
                : null;

            if (_takeItemAction.action.WasPressedThisFrame())
                TryTakeItem();

            if (_dropItemAction.action.WasPressedThisFrame())
                TryDropItem();

            if (_grabReleaseItemAction.action.WasPressedThisFrame())
            {
                if (_currentGrabbedItem is null)
                    TryGrabItem();
                else
                    TryReleaseItem();
            }

            UpdateSelectedItem();
        }

        private void UpdateSelectedItem ()
        {
            if (_inventory.Count == 0)
                return;

            var delta = _invenoryIndexDelta.action.ReadValue<Vector2>().y;
            if (delta > 0.0f)
            {
                InventoryIndex = (InventoryIndex + 1) % _inventory.Count;
            }
            if (delta < 0.0f)
            {
                InventoryIndex = (InventoryIndex - 1) < 0 ? _inventory.Count - 1 : InventoryIndex - 1;
            }
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            if (_obstacleLayerMask.value == LayerMask.GetMask(NothingLayerName))
                _obstacleLayerMask = LayerMask.GetMask(DefaultLayerName, ItemsLayerName, ItemsSlotsLayerName);
            _indexSocket = new(_inventoryIndex);
            _selectedItemSocket = new(SelectedItem);

            _isInited = true;
        }
    }
}