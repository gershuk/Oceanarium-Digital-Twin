#nullable enable

using System;

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.Items
{
    public enum ItemState
    {
        Free = 0,
        Picked = 1,
    }

    [Serializable]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Item : InfoObject
    {
        #region Layers names
        public const string DefaultLayerName = "Default";
        public const string IgnoreRaycastLayerName = "Ignore Raycast";
        public const string ItemsLayerName = "Items";
        public const string ItemsSlotsLayerName = "ItemSlots";
        public const string NothingLayerName = "Nothing";
        #endregion Layers names

        private MulticonnectionSocket<ItemState, ItemState> _stateSocket;

        public IOutputSocket<ItemState> StateSocket => _stateSocket;

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
            _stateSocket = new (ItemState.Free);
        }

        public void Take (bool isActive = true)
        {
            _stateSocket.TrySetValue(ItemState.Picked);
            GameObject.layer = LayerMask.NameToLayer(IgnoreRaycastLayerName);
            Collider.enabled = false;
            Rigidbody.isKinematic = true;
            GameObject.SetActive(isActive);
        }

        public void Drop ()
        {
            _stateSocket.TrySetValue(ItemState.Free);
            GameObject.layer = LayerMask.NameToLayer(ItemsLayerName);
            Collider.enabled = true;
            Rigidbody.isKinematic = false;
            GameObject.SetActive(true);
        }
    }
}