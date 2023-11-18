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
        _itemScript = itemScript;
    }

    public void RemoveItem ()
    {
        _itemScript = null;
    }
}
