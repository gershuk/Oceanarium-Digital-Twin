using UnityEngine;

public class ItemScript : MonoBehaviour, IInfo
{
    private Rigidbody _rb;
    private Collider _collider;

    public string itemName = "item";
    public string itemDescription = "item";

    public string Name => itemName;

    public string Description => itemDescription;

    public Rigidbody Rigidbody => _rb;

    public Collider Collider => _collider;

    public GameObject GameObject => gameObject;

    private void Awake ()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
}