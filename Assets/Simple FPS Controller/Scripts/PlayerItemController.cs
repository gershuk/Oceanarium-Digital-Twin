using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemController : MonoBehaviour
{
    private IInfo _currentObservedObject;
    private ItemScript _currentGrabbedItem = null;
    private List<ItemScript> _inventory;
    private int _inventoryIndex = 0;

    public Camera _camera;
    public int inventorySize = 9;

    public float distanceOfItemInteraction = 5;
    public float distanceOfItemDrop = 5;
    public LayerMask obstacleLayerMask;

    [Header("Input")]
    public InputActionReference takeItemAction;
    public InputActionReference dropItemAction;
    public InputActionReference gradReleaseItemAction;
    public InputActionReference invenoryIndexdelta;
    public InputActionReference inventoyPrev;

    public IInfo SelectedItem => _inventoryIndex == -1 ? null : _inventory[_inventoryIndex];

    private void Awake ()
    {
        _inventory = new List<ItemScript>(inventorySize);
    }

    private void Update ()
    {
        _currentObservedObject = Physics.Raycast(_camera.transform.position,
                                               _camera.transform.TransformDirection(Vector3.forward),
                                               out var hit,
                                               distanceOfItemInteraction,
                                               obstacleLayerMask)
            ? hit.transform.gameObject.GetComponent<IInfo>()
            : null;

        if (takeItemAction.action.WasPressedThisFrame())
            TryTakeItem();

        if (dropItemAction.action.WasPressedThisFrame())
            TryDropItem();

        if (gradReleaseItemAction.action.WasPressedThisFrame())
        {
            if (_currentGrabbedItem is null)
                TryGrabItem();
            else
                TryReleaseItem();
        }
    }

    private bool TryGrabItem ()
    {
        if (_currentObservedObject as ItemScript is not null)
        {
            _currentGrabbedItem = (ItemScript) _currentObservedObject;
            _currentObservedObject = null;
        }
        if (_currentObservedObject as ItemSlotScript is not null)
        {
            var itemSlot = (ItemSlotScript) _currentObservedObject;
            _currentGrabbedItem = itemSlot.CurrentItem;
            if (_currentGrabbedItem is not null)
                itemSlot.RemoveItem();
        }
        if (_currentGrabbedItem is not null)
        {
            _currentGrabbedItem.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            _currentGrabbedItem.transform.parent = _camera.transform;
            _currentGrabbedItem.Rigidbody.isKinematic = true;
            return true;
        }
        else
            return false;
    }

    private bool TryReleaseItem ()
    {
        if (_currentGrabbedItem is not null)
        {
            _currentGrabbedItem.gameObject.layer = LayerMask.NameToLayer("Items");
            _currentGrabbedItem.transform.parent = null;
            _currentGrabbedItem.Rigidbody.isKinematic = false;
            _currentGrabbedItem = null;
            return true;
        }
        return false;
    }

    private bool TryTakeItem ()
    {
        if (_currentObservedObject is not null && _inventory.Count < inventorySize)
        {
            switch (_currentObservedObject)
            {
                case ItemScript itemScript:
                    itemScript.Rigidbody.isKinematic = true;
                    itemScript.gameObject.SetActive(false);
                    _inventory.Add(itemScript);
                    _currentObservedObject = null;
                    break;
                case ItemSlotScript itemSlotScript:
                    var itemInSlot = itemSlotScript.CurrentItem;
                    if (itemInSlot is not null)
                    {
                        _inventory.Add(itemInSlot);
                        itemSlotScript.RemoveItem();
                        itemInSlot.gameObject.SetActive(false);
                    }
                    break;
            }
            _inventoryIndex = _inventory.Count - 1;
            return true;
        }
        return false;
    }

    private bool TryDropItem ()
    {
        if (_inventoryIndex >= _inventory.Count || _inventoryIndex < 0)
            return false;

        var item = _inventory[_inventoryIndex];

        if (!Physics.Raycast(_camera.transform.position,
                            _camera.transform.TransformDirection(Vector3.forward),
                            out var hit,
                            distanceOfItemDrop,
                            obstacleLayerMask))
        {
            item.transform.rotation = _camera.transform.rotation;
            item.transform.position = _camera.transform.TransformPoint(Vector3.forward * distanceOfItemDrop);
            item.GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            switch (hit.transform.gameObject.GetComponent<IInfo>())
            {
                case ItemSlotScript itemSlotScript:
                    if (itemSlotScript.CurrentItem is null)
                    {
                        itemSlotScript.SetItem(item);
                    }
                    else
                        return false;
                    break;
                default:
                    item.transform.rotation = _camera.transform.rotation;
                    item.transform.position = hit.point + hit.normal;
                    item.GetComponent<Rigidbody>().isKinematic = false;
                    break;
            }
        }
        _inventory.RemoveAt(_inventoryIndex);
        item.gameObject.SetActive(true);
        if (_inventoryIndex == _inventory.Count)
            _inventoryIndex--;
        return true;
    }
}