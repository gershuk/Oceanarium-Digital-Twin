#nullable enable

using System;

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.Items
{
    [Serializable]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Item : InfoObject
    {
        public ItemSpawner? Spawner { get => _spawner; set => _spawner = value; }

        #region Item start parameters
        [SerializeField]
        private ItemSpawner? _spawner;

        #endregion Item start parameters

        public Rigidbody Rigidbody { get; private set; }

        public override bool TryResetPosition ()
        {
            switch (_spawner, _defaultRespawnPosition)
            {
                case (not null, _): 
                    _spawner.ResetSpawnedItemPosition(this);
                    return true;
                case (_, not null):
                    return base.TryResetPosition();
                default: 
                    return false;
            };
        }

        protected override void SubInit ()
        {
            base.SubInit();
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}