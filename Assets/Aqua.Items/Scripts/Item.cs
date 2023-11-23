using UnityEngine;

namespace Aqua.Items
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Item : MonoBehaviour, IInfo
    {
        public string itemDescription = "item";
        public string itemName = "item";
        public Collider Collider { get; private set; }
        public string Description => itemDescription;
        public GameObject GameObject { get; private set; }
        public string Name => itemName;
        public Rigidbody Rigidbody { get; private set; }

        private void Awake ()
        {
            GameObject = GetComponent<GameObject>();
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();
        }
    }
}