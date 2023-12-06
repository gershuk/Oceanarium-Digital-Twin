#nullable enable

using System;

using Aqua.SocketSystem;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Aqua.FPSController
{
    [Serializable]
    public enum PlayerControllerState
    {
        None = 0,
        MovementInput = 1,
        Cursor = 2,
        Menu = 3,
    }

    public sealed class PlayerModel : MonoBehaviour
    {
        private MulticonnectionSocket<PlayerControllerState, PlayerControllerState> _stateSocket;
        public IOutputSocket<PlayerControllerState> StateSocket => _stateSocket;

        [SerializeField]
        private FPSCamera _fpsCamera;

        [SerializeField]
        private PlayerInventory _inventory;

        private bool _isInited = false;

        [SerializeField]
        private PlayerMovement _playerMovement;

        [SerializeField]
        private PlayerControllerState _startState = PlayerControllerState.None;

        public FPSCamera FpsCamera { get => _fpsCamera; private set => _fpsCamera = value; }
        public PlayerInventory Inventory { get => _inventory; private set => _inventory = value; }
        public PlayerMovement PlayerMovement { get => _playerMovement; private set => _playerMovement = value; }

        public PlayerControllerState State
        {
            get => _stateSocket.GetValue();
            set
            {
                if (!_stateSocket.TrySetValue(value))
                {
                    Debug.LogWarning("Can't set value when socket has input connection");
                    return;
                }

                switch (_stateSocket.GetValue())
                {
                    case PlayerControllerState.None:
                        IsMovementInputAcitve = false;
                        IsCursorAcitve = false;
                        IsCameraAcitve = false;
                        break;
                    case PlayerControllerState.MovementInput:
                        IsMovementInputAcitve = true;
                        IsCursorAcitve = false;
                        IsCameraAcitve = true;
                        break;
                    case PlayerControllerState.Cursor:
                        IsMovementInputAcitve = false;
                        IsCursorAcitve = true;
                        IsCameraAcitve = true;
                        break;
                    case PlayerControllerState.Menu:
                        IsMovementInputAcitve = false;
                        IsCursorAcitve = true;
                        IsCameraAcitve = true;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #region Input actions

        [Header("Input actions")]
        [SerializeField]
        private InputActionReference _showHideMenu;

        [SerializeField]
        private InputActionReference _showHideCursor;
        private bool _isMovementInputAcitve;
        private bool _isCursorAcitve;
        private bool _isCameraAcitve;

        #endregion

        private void Awake ()
        {
            ForceInit();
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            FindObjectsIfNull();

            _stateSocket = new(PlayerControllerState.MovementInput);

            SubscribeOnActions();

            State = _startState;

            _isInited = true;
        }

        private void FindObjectsIfNull ()
        {
            if (_fpsCamera == null)
                _fpsCamera = GetComponent<FPSCamera>();

            if (_inventory == null)
                _inventory = GetComponent<PlayerInventory>();

            if (_playerMovement == null)
                _playerMovement = GetComponent<PlayerMovement>();
        }

        private void OnDestroy ()
        {
            UnsubscribeFromActions();
        }

        private void SubscribeOnActions ()
        {
            _showHideCursor.action.performed += OnShowHideCursorPerformed;
            _showHideMenu.action.performed += OnShowHideMenuPerformed;
        }

        private void UnsubscribeFromActions ()
        {
            _showHideCursor.action.performed -= OnShowHideCursorPerformed;
            _showHideMenu.action.performed -= OnShowHideMenuPerformed;
        }

        private void OnShowHideMenuPerformed (InputAction.CallbackContext obj) => State = State switch
        {
            PlayerControllerState.None => PlayerControllerState.None,
            PlayerControllerState.MovementInput => PlayerControllerState.Menu,
            PlayerControllerState.Cursor => PlayerControllerState.Menu,
            PlayerControllerState.Menu => PlayerControllerState.MovementInput,
            _ => throw new NotImplementedException(),
        };

        private void OnShowHideCursorPerformed (InputAction.CallbackContext obj) => State = State switch
        {
            PlayerControllerState.None => PlayerControllerState.None,
            PlayerControllerState.MovementInput => PlayerControllerState.Cursor,
            PlayerControllerState.Cursor => PlayerControllerState.MovementInput,
            PlayerControllerState.Menu => PlayerControllerState.Menu,
            _ => throw new NotImplementedException(),
        };

        public bool IsCursorAcitve
        {
            get => _isCursorAcitve;
            private set
            {
                _isCursorAcitve = value;
                Cursor.visible = _isCursorAcitve;
                Cursor.lockState = _isCursorAcitve ? CursorLockMode.Confined : CursorLockMode.Locked;
            }
        }

        public bool IsMovementInputAcitve
        {
            get => _isMovementInputAcitve;
            private set
            {
                _isMovementInputAcitve = value;
                _fpsCamera.enabled = _isMovementInputAcitve;
                PlayerMovement.enabled = _isMovementInputAcitve;
            }
        }

        public bool IsCameraAcitve
        {
            get => _isCameraAcitve;
            private set
            {
                _isCameraAcitve = value;
                FpsCamera.gameObject.SetActive(_isCameraAcitve);
            }
        }
    }
}