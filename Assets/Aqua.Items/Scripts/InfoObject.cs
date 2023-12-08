#nullable enable

using System;

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.Items
{
    [Serializable]
    [RequireComponent(typeof(Collider))]
    public class InfoObject : MonoBehaviour, IInfo
    {
        protected static Sprite? _deafultSprite;

        #region Item start parameters

        [Header("Item start parameters")]
        [SerializeField]
        protected string _descritption = "description";

        [SerializeField]
        protected string _name = "item";

        [SerializeField]
        protected Sprite _sprite;

        [SerializeField]
        protected Transform _defaultRespawnPosition;

        #endregion Item start parameters

        #region Sockets
        protected MulticonnectionSocket<string, string> _descriptionSocket { get; set; }
        protected MulticonnectionSocket<string, string> _nameSocket { get; set; }
        protected MulticonnectionSocket<Sprite, Sprite> _spriteSocket { get; set; }

        public IOutputSocket<string> DescriptionSocket => _descriptionSocket;

        public IOutputSocket<string> NameSocket => _nameSocket;

        public IOutputSocket<Sprite> SpriteSocket => _spriteSocket;
        #endregion Sockets

        public Collider Collider { get; protected set; }

        protected bool _isInited = false;

        public GameObject GameObject { get; protected set; }

        protected void Awake ()
        {
            ForceInit();
        }

        public virtual bool TryResetPosition ()
        {
            if (_defaultRespawnPosition != null)
            {
                transform.position = _defaultRespawnPosition.position;
                transform.rotation = _defaultRespawnPosition.rotation;
            }

            return _defaultRespawnPosition;
        }

        protected virtual void SubInit ()
        {
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
            Collider = GetComponent<Collider>();
        }

        public  void ForceInit ()
        {
            if (_isInited)
                return;

           SubInit();

            _isInited = true;
        }
    }
}
