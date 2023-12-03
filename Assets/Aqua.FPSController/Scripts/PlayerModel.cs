using UnityEngine;

namespace Aqua.FPSController
{
    public sealed class PlayerModel : MonoBehaviour
    {
        [SerializeField]
        private FPSCamera _fpsCamera;

        [SerializeField]
        private PlayerInventory _inventory;

        private bool _isInited = false;

        [SerializeField]
        private PlayerMovement _playerMovement;

        public FPSCamera FpsCamera { get => _fpsCamera; private set => _fpsCamera = value; }
        public PlayerInventory Inventory { get => _inventory; private set => _inventory = value; }
        public PlayerMovement PlayerMovement { get => _playerMovement; private set => _playerMovement = value; }

        private void Awake ()
        {
            ForceInit();
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            if (_fpsCamera == null)
                _fpsCamera = GetComponent<FPSCamera>();

            if (_inventory == null)
                _inventory = GetComponent<PlayerInventory>();

            if (_playerMovement == null)
                _playerMovement = GetComponent<PlayerMovement>();

            _isInited = true;
        }
    }
}