using UnityEngine;

namespace Aqua.Items
{
    public class ItemSlot : MonoBehaviour, IInfo
    {
        [SerializeField]
        protected Transform _itemPosition;

        protected Item _itemScript = null;

        [SerializeField]
        protected string _slotDescription = "slot";

        [SerializeField]
        protected string _slotName = "slot";

        public const string IgnorRayCastLayerName = "Ignore Raycast";
        public const string ItemsLayerName = "Items";
        public Item CurrentItem => _itemScript;
        public string Description => _slotDescription;
        public string Name => _slotName;

        protected void OnTriggerEnter (Collider other)
        {
            if (CurrentItem != null)
            {
                Debug.Log($"Slot is not empty. Item '{CurrentItem}' already attached");
                return;
            }
            var item = other.GetComponent<Item>();
            if (item != null)
                SetItem(item);
        }

        public void RemoveItem ()
        {
            _itemScript.gameObject.layer = LayerMask.NameToLayer(ItemsLayerName);
            _itemScript = null;
        }

        public void SetItem (Item itemScript)
        {
            itemScript.transform.position = transform.position;
            itemScript.transform.rotation = transform.rotation;
            itemScript.gameObject.layer = LayerMask.NameToLayer(IgnorRayCastLayerName);
            itemScript.Rigidbody.isKinematic = true;
            _itemScript = itemScript;
        }
    }
}