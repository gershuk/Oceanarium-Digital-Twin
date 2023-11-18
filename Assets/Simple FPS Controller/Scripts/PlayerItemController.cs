using System;
using System.Collections.Generic;

using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    private ItemScript _currentObservedItem;
    private ItemScript _currentGrabbedItem = null;
    private List<ItemScript> _inventory;
    private int _inventoryIndex = 0;

    public Camera _camera;
    public int inventorySize = 9;

    public float distanceOfItemInteraction = 5;
    public float distanceOfItemDrop = 5;
    public LayerMask obstacleLayerMask;

    private void Awake ()
    {
        _inventory = new List<ItemScript>(inventorySize);
    }

    private void Update ()
    {
        _currentObservedItem =
            Physics.Raycast(_camera.transform.position,
                            _camera.transform.TransformDirection(Vector3.forward),
                            out var hit,
                            distanceOfItemInteraction,
                            obstacleLayerMask)
            ? hit.transform.gameObject.GetComponent<ItemScript>()
            : null;

        if (Input.GetMouseButtonDown(0))
        {
            TryTakeItem();
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryDropItem();
        }

        if (Input.GetMouseButtonDown(2))
        {
            if (_currentGrabbedItem is null)
            {
                TryGrabItem();
            }
            else
            {
                TryReleaseItem();
            }
        }
    }

    private bool TryGrabItem ()
    {
        if (_currentObservedItem is not null)
        {
            _currentGrabbedItem = _currentObservedItem;
            _currentObservedItem = null;
            _currentGrabbedItem.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            _currentGrabbedItem.transform.parent = _camera.transform;
            _currentGrabbedItem.Rigidbody.isKinematic = true;
            return true;
        }
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
        if (_currentObservedItem is not null && _inventory.Count < inventorySize)
        {
            _currentObservedItem.GetComponent<Rigidbody>().isKinematic = true;
            _currentObservedItem.gameObject.SetActive(false);
            _inventory.Add(_currentObservedItem);
            _currentObservedItem = null;
            return true;
        }
        return false;
    }

    private bool TryDropItem ()
    {
        if (_inventoryIndex < _inventory.Count)
        {
            var item = _inventory[_inventoryIndex];
            _inventory.RemoveAt(_inventoryIndex);

            item.transform.position = Physics.Raycast(_camera.transform.position,
                                _camera.transform.TransformDirection(Vector3.forward),
                                out var hit,
                                distanceOfItemDrop,
                                obstacleLayerMask)
                ? hit.point + hit.normal    // TODO Need more correct position defining
                : _camera.transform.TransformPoint(Vector3.forward * distanceOfItemDrop);

            item.transform.rotation = _camera.transform.rotation;
            item.GetComponent<Rigidbody>().isKinematic = false;
            item.gameObject.SetActive(true);
            return true;
        }
        return false;
    }
}