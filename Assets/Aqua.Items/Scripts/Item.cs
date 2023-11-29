#nullable enable

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.Items
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Item : MonoBehaviour, IInfo
    {
        private static Sprite? _deafultSprite;

        #region Item start parameters
        [Header("Item start parameters")]
        [SerializeField]
        private string _name = "item";
        [SerializeField]
        private string _descritption = "description";
        [SerializeField]
        private Sprite _sprite;
        #endregion

        public Collider Collider { get; private set; }
        public GameObject GameObject { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        #region Sockets
        private MulticonnectionSocket<string, string> _nameSocket { get; set; }

        private MulticonnectionSocket<string, string> _descriptionSocket { get; set; }

        private MulticonnectionSocket<Sprite, Sprite> _spriteSocket { get; set; }

        public IOutputSocket<string> DescriptionSocket => _descriptionSocket;

        public IOutputSocket<string> NameSocket => _nameSocket;

        public IOutputSocket<Sprite> SpriteSocket => _spriteSocket;
        #endregion

        public Item ()
        {
            _nameSocket = new(_name);
            _descriptionSocket = new(_descritption);
            _spriteSocket = new(_sprite);
        }

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

            GameObject = GetComponent<GameObject>();
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();           
        }
    }
}