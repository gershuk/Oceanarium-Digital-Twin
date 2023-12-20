#nullable enable

using System.Linq;

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.Items
{
    [RequireComponent(typeof(Collider))]
    public class ItemSlot : MonoBehaviour, IInfo
    {
        protected bool _isInited;

        protected static Sprite? _deafultSprite;

        #region Item slot start parameters
        [Header("Item slot start parameters")]
        [SerializeField]
        protected string _descritption;

        [SerializeField]
        protected string _name;

        [SerializeField]
        protected Sprite _sprite;

        [SerializeField]
        protected string[] _requiredNames;

        [SerializeField]
        protected Item _defaultItem;

        #endregion Item slot start parameters

        #region Sockets
        protected MulticonnectionSocket<string, string> _descriptionSocket { get; set; }
        protected MulticonnectionSocket<string, string> _nameSocket { get; set; }
        protected MulticonnectionSocket<Sprite, Sprite> _spriteSocket { get; set; }
        protected MulticonnectionSocket<Item?,Item?> _itemSocket { get; set; }

        public IOutputSocket<string> DescriptionSocket => _descriptionSocket;

        public IOutputSocket<string> NameSocket => _nameSocket;

        public IOutputSocket<Sprite> SpriteSocket => _spriteSocket;
        public IOutputSocket<Item?> ItemSocket => _itemSocket;
        #endregion Sockets

        [SerializeField]
        protected Transform _itemPosition;

        protected Transform Anchor => _itemPosition == null ? transform : _itemPosition;

        public const string IgnorRayCastLayerName = "Ignore Raycast";
        public const string ItemsLayerName = "Items";
        public Item? CurrentItem => _itemSocket.GetValue();

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

            _itemSocket = new();

            if (_defaultItem != null)
            {
                _defaultItem.ForceInit();
                TrySetItem(_defaultItem);
            }

            _isInited = true;
        }

        protected void Awake () => ForceInit();

        protected void OnTriggerEnter (Collider other)
        {
            if (!other.TryGetComponent<Item>(out var item))
                return;

            if (CurrentItem != null)
            {
                Debug.Log($"Slot is not empty. Item '{CurrentItem}' already attached. Slot {gameObject.name}.");
                return;
            }

            TrySetItem(item);
        }

        protected void OnTriggerExit (Collider other)
        {
            var item = other.GetComponent<Item>();

            if (item != null && CurrentItem == item)
                TakeItem();
        }

        public Item? TakeItem ()
        {
            var item = _itemSocket.GetValue();
            if (item != null)
            {
                RemoveItemFromSlot();
                item.Take(false);
            }
            return item;
        }

        public void RemoveItemFromSlot ()
        {
            if (_itemSocket.GetValue() == null)
            {
                Debug.LogError("Item is not setted");
                return;
            }
            _itemSocket.GetValue()!.gameObject.layer = LayerMask.NameToLayer(ItemsLayerName);
            _itemSocket.TrySetValue(null);
        }

        public bool TrySetItem (Item item)
        {
            if (_requiredNames.Length > 0 && !_requiredNames.Contains(item.NameSocket.GetValue()))
            {
                Debug.Log("Wrang item name. Can't set it.");
                return false;
            }

            if (_itemSocket.GetValue() != null)
            {
                Debug.Log($"Can't set item to slot '{NameSocket.GetValue()}'. It's not empty.");
                return false;
            }

            item.transform.SetPositionAndRotation(Anchor.position, Anchor.rotation);

            item.Take(true);
            if (!_itemSocket.TrySetValue(item))
            {
                Debug.LogError("Can't set value when socket has input link.");
            }

            return true;
        }
    }
}