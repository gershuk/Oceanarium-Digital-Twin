#nullable enable

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.Items
{
    [RequireComponent(typeof(Collider))]
    public class ItemSlot : MonoBehaviour, IInfo
    {
        private bool _isInited;

        private static Sprite? _deafultSprite;

        #region Item slot start parameters
        [Header("Item slot start parameters")]
        [SerializeField]
        private string _descritption;

        [SerializeField]
        private string _name;

        [SerializeField]
        private Sprite _sprite;

        [SerializeField]
        private string _requiredName;

        [SerializeField]
        private Item _defaultItem;

        #endregion Item slot start parameters

        #region Sockets
        private MulticonnectionSocket<string, string> _descriptionSocket { get; set; }
        private MulticonnectionSocket<string, string> _nameSocket { get; set; }
        private MulticonnectionSocket<Sprite, Sprite> _spriteSocket { get; set; }

        public IOutputSocket<string> DescriptionSocket => _descriptionSocket;

        public IOutputSocket<string> NameSocket => _nameSocket;

        public IOutputSocket<Sprite> SpriteSocket => _spriteSocket;
        #endregion Sockets

        protected Item? _item = null;

        [SerializeField]
        protected Transform _itemPosition;

        private Transform Anchor => _itemPosition == null ? transform : _itemPosition;

        public const string IgnorRayCastLayerName = "Ignore Raycast";
        public const string ItemsLayerName = "Items";
        public Item CurrentItem => _item;

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _nameSocket = new(_name);
            _descriptionSocket = new(_descritption);
            _spriteSocket = new(_sprite);

            if (_spriteSocket.GetValue() == null)
            {
                if (_deafultSprite == null)
                {
                    _deafultSprite = Resources.Load<Sprite>(DefaultResourcePathes.DefaultItemSpritePath);
                }
                _spriteSocket.TrySetValue(_deafultSprite);
            }

            _isInited = true;

            if (_defaultItem != null)
            {
                _defaultItem.ForceInit();
                SetItem(_defaultItem);
            }
        }

        protected void Awake ()
        {
            ForceInit();
        }

        protected void OnTriggerEnter (Collider other)
        {
            var item = other.GetComponent<Item>();

            if (item == null)
                return;

            if (CurrentItem != null)
            {
                Debug.Log($"Slot is not empty. Item '{CurrentItem}' already attached");
                return;
            }

            SetItem(item);
        }

        protected void OnTriggerExit (Collider other)
        {
            var item = other.GetComponent<Item>();

            if (item != null && CurrentItem == item)
                TakeItem();
        }

        public Item? TakeItem ()
        {
            var item = _item;
            if (item != null)
                RemoveItem();
            return item;
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

        public void SetItem (Item item)
        {
            item.transform.position = Anchor.position;
            item.transform.rotation = Anchor.rotation;

            if (!string.IsNullOrEmpty(_requiredName) && _requiredName != item.NameSocket.GetValue())
            {
                Debug.Log("Wrang item name. Can't set it.");
                return;
            }
            
            item.gameObject.layer = LayerMask.NameToLayer(IgnorRayCastLayerName);
            item.Rigidbody.isKinematic = true;
            _item = item;
        }
    }
}