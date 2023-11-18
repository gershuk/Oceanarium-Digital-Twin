using UnityEngine;

public class ItemScript : MonoBehaviour, IItemInfo
{
    private Rigidbody _rb;
    private Collider _collider;

    public string itemName = "test";
    public string itemDescription = "test";

    public string Name => itemName;

    public string Description => itemDescription;

    public Rigidbody Rigidbody => _rb;

    public Collider Collider => _collider;

    private void Awake ()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
}