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
            var intObj = GetInteractableObjectFromInfo(_objectScaner.ObservedObjectSocket.GetValue());
            if ((_doProcessingTime + _doProcessingCooldown) <= Time.time &&  intObj != null)
            {
                intObj.DoProcessingAction();
                return true;
            }

            return false;
        }

        private bool TryUndoProcessing ()
        {
            var intObj = GetInteractableObjectFromInfo(_objectScaner.ObservedObjectSocket.GetValue());
            if ((_undoProcessingTime + _undoProcessingCooldown) <= Time.time && intObj != null)
            {
                intObj.UndoProcessingAction();
                return true;
            }

            return false;
        }

        private bool TryUse ()
        {
            var intObj = GetInteractableObjectFromInfo(_objectScaner.ObservedObjectSocket.GetValue());
            if ((_useTime + _useCooldown) <= Time.time && intObj != null)
            {
                intObj.Use();
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

        protected IInteractableObject? GetInteractableObjectFromInfo (IInfo? info) =>
            info is MonoBehaviour behaviour and not null
            && behaviour.gameObject.GetComponent<IInteractableObject>() is var interactableObject and not null
                ? interactableObject
                : null;

        public void ForceInit ()
        {
            if (_isInited)
                return;

            InitTime();

            _isInited = true;
        }
    }
}