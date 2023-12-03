#nullable enable

using System;

using Aqua.SocketSystem;

using UnityEngine;
using UnityEngine.UI;

using UniRx;

namespace Aqua.UIBaseElements
{
    [Serializable]
    public enum AimState
    {
        Normal = 0,
        Highlighted = 1,
    }

    public class HUDAimViewModel : MonoBehaviour
    {
        [SerializeField]
        private Image _aimImage;

        [SerializeField]
        private Color _highlightedColor;

        private bool _isInited = false;

        [SerializeField]
        private Color _normalColor;

        private UniversalSocket<AimState, AimState> _stateSocket;

        public AimState State
        {
            get => _stateSocket.GetValue();
            set
            {
                if (!_stateSocket.TrySetValue(value))
                {
                    Debug.LogWarning("Can't set value if when socket has input connection");
                }
            }
        }

        public IUniversalSocket<AimState,AimState> StateSocket => _stateSocket;

        private void UpdateGraphicalState (AimState state)
        {
            switch (state)
            {
                case AimState.Normal:
                    _aimImage.color = _normalColor;
                    break;

                case AimState.Highlighted:
                    _aimImage.color = _highlightedColor;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void Awake ()
        {
            ForceInit();
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            if (_aimImage == null)
                _aimImage = GetComponent<Image>();

            _stateSocket = new(AimState.Normal);
            _stateSocket.ReadOnlyProperty.Subscribe(UpdateGraphicalState).AddTo(this);

            _isInited = true;
        }
    }
}