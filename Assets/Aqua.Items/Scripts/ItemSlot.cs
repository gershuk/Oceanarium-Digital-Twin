#nullable enable

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.Items
{
    public class ItemSlot : MonoBehaviour, IInfo
    {
        private static Sprite? _deafultSprite;

        #region Item slot start parameters
        [Header("Item slot start parameters")]
        [SerializeField]
        private string _name = "item";
        [SerializeField]
        private string _descritption = "description";
        [SerializeField]
        private Sprite _sprite;
        #endregion

        #region Sockets
        private MulticonnectionSocket<string, string> _nameSocket { get; set; }

        private MulticonnectionSocket<string, string> _descriptionSocket { get; set; }

        private MulticonnectionSocket<Sprite, Sprite> _spriteSocket { get; set; }

        public IOutputSocket<string> DescriptionSocket => _descriptionSocket;

        public IOutputSocket<string> NameSocket => _nameSocket;

        public IOutputSocket<Sprite> SpriteSocket => _spriteSocket;
        #endregion

        [SerializeField]
        protected Transform _itemPosition;

        protected Item? _item = null;

        [SerializeField]
        protected string _slotDescription = "slot";

        [SerializeField]
        protected string _slotName = "slot";

        public const string IgnorRayCastLayerName = "Ignore Raycast";
        public const string ItemsLayerName = "Items";
        public Item CurrentItem => _item;
        public string Description => _slotDescription;
        public string Name => _slotName;

        protected void Awake ()
        {
            if (_spriteSocket.GetValue() == null)
            {
                if (_deafultSprite == null)
                {
                    _deafultSprite = Resources.Load<Sprite>(DefaultResourcePathes.DefaultItemSpritePath);
                }
                _spriteSocket.TrySetValue(_deafultSprite);
            }
        }

        public ItemSlot ()
        {
            _nameSocket = new(_name);
            _descriptionSocket = new(_descritption);
            _spriteSocket = new(_sprite);
        }

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
            if (_item == null)
            {
                Debug.LogError("Item is not setted");
                return;
            }
            _item.gameObject.layer = LayerMask.NameToLayer(ItemsLayerName);
            _item = null;
        }

        public void SetItem (Item itemScript)
        {
            itemScript.transform.position = transform.position;
            itemScript.transform.rotation = transform.rotation;
            itemScript.gameObject.layer = LayerMask.NameToLayer(IgnorRayCastLayerName);
            itemScript.Rigidbody.isKinematic = true;
            _item = itemScript;
        }
    }
}