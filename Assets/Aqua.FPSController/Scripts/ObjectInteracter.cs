#nullable enable

using Aqua.Items;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Aqua.FPSController
{
    public class ObjectInteracter : MonoBehaviour
    {
        private bool _isInited = false;

        [SerializeField]
        private ObjectScaner _objectScaner;

        #region Input actions

        [Header("Input actions")]
        [SerializeField]
        private InputActionReference _doProcessingAction;

        [SerializeField]
        private InputActionReference _undoProcessingAction;

        [SerializeField]
        private InputActionReference _useAction;

        #endregion Input actions

        #region Action cooldowns

        [Header("Action cooldowns")]
        [SerializeField]
        private float _doProcessingCooldown;

        private float _doProcessingTime;

        [SerializeField]
        private float _undoProcessingCooldown;

        private float _undoProcessingTime;

        [SerializeField]
        private float _useCooldown;

        private float _useTime;
        #endregion Action cooldowns

        private void InitTime ()
        {
            _useTime = 0;
            _doProcessingTime = 0;
            _undoProcessingTime = 0;
        }

        private bool TryDoProcessing ()
        {
            if ((_doProcessingTime + _doProcessingCooldown) <= Time.time
                && _objectScaner.ObservedObjectSocket.GetValue() is IInteractableObject interactableObject and not null)
            {
                interactableObject.DoProcessingAction();
                return true;
            }

            return false;
        }

        private bool TryUndoProcessing ()
        {
            if ((_undoProcessingTime + _undoProcessingCooldown) <= Time.time
                && _objectScaner.ObservedObjectSocket.GetValue() is IInteractableObject interactableObject and not null)
            {
                interactableObject.UndoProcessingAction();
                return true;
            }

            return false;
        }

        private bool TryUse ()
        {
            if ((_useTime + _useCooldown) <= Time.time
                && _objectScaner.ObservedObjectSocket.GetValue() is IInteractableObject interactableObject and not null)
            {
                interactableObject.Use();
                return true;
            }

            return false;
        }

        private void Update ()
        {
            if (_useAction.action.IsPressed())
                TryUse();

            if (_doProcessingAction.action.IsPressed())
                TryDoProcessing();

            if (_undoProcessingAction.action.IsPressed())
                TryUndoProcessing();
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            InitTime();

            _isInited = true;
        }
    }
}