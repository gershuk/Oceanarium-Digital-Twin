using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ItemSlotScript : MonoBehaviour, IInfo
{
    public Transform itemPosition;

    public string slotName = "slot";
    public string slotDescription = "slot";

    public string Name => slotName;
    public string Description => slotDescription;
    public GameObject GameObject => gameObject;

    private ItemScript _itemScript = null;

    public ItemScript CurrentItem => _itemScript;

    public void SetItem (ItemScript itemScript)
    {
        itemScript.transform.position = transform.position;
        itemScript.transform.rotation = transform.rotation;
        itemScript.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        itemScript.Rigidbody.isKinematic = true;
        _itemScript = itemScript;
    }

    public void RemoveItem ()
    {
        _itemScript.gameObject.layer = LayerMask.NameToLayer("Items");
        _itemScript = null;
    }

    private void OnTriggerEnter (Collider other)
    {
        if (CurrentItem is not null)
            return;
        var item = other.GetComponent<ItemScript>();
        if (item is not null)
            SetItem(item);
    }
}
