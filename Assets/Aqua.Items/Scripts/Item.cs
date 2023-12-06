#nullable enable

using System;

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.Items
{
    [Serializable]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Item : MonoBehaviour, IInfo
    {
        private bool _isInited = false;

        private static Sprite? _deafultSprite;

        #region Item start parameters

        [Header("Item start parameters")]
        [SerializeField]
        private string _descritption = "description";
    
        [SerializeField]
        private string _name = "item";

        [SerializeField]
        private Sprite _sprite;

        #endregion Item start parameters

        public Collider Collider { get; private set; }
        public GameObject GameObject { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        #region Sockets
        private MulticonnectionSocket<string, string> _descriptionSocket { get; set; }
        private MulticonnectionSocket<string, string> _nameSocket { get; set; }
        private MulticonnectionSocket<Sprite, Sprite> _spriteSocket { get; set; }

        public IOutputSocket<string> DescriptionSocket => _descriptionSocket;

        public IOutputSocket<string> NameSocket => _nameSocket;

        public IOutputSocket<Sprite> SpriteSocket => _spriteSocket;
        #endregion Sockets

        protected void Awake ()
        {
            ForceInit();
        }

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

            GameObject = GetComponent<GameObject>();
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();

            _isInited = true;
        }
    }
}