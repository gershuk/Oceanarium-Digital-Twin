using System;
using System.Collections.Generic;

using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    private Camera _cam;
    private ItemScript? _currentObservedItem;

    public float distance = 5;
    public LayerMask layerMask = 1;

    private void Awake ()
    {
        _cam = this.GetComponent<Camera>();
    }

    private void Update ()
    {
        if (Physics.Raycast(_cam.transform.position, _cam.transform.TransformDirection(Vector3.forward), out var hit, distance, layerMask))
        {
            ItemScript itemInfo = hit.transform.gameObject.GetComponent<ItemScript>();
            if (itemInfo is not null)
            {
                _currentObservedItem = itemInfo;
                //Debug.Log(itemInfo.InfoLable);
            }
        }
    }
}