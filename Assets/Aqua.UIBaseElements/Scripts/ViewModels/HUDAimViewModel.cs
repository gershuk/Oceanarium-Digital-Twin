#nullable enable

using System;

using Aqua.Items;
using Aqua.SocketSystem;

using TMPro;

using UniRx;

using UnityEngine;
using UnityEngine.UI;

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
        private readonly CompositeDisposable _disposables = new();

        [SerializeField]
        private Image _aimImage;

        [SerializeField]
        private Color _highlightedColor;

        private UniversalSocket<IInfo, IInfo> _infoSocket;

        private bool _isInited = false;

        [SerializeField]
        private Color _normalColor;

        [SerializeField]
        private TMP_Text _objectName;

        private ConverterSocket<IInfo, AimState> _stateSocket;
        public IInputSocket<IInfo> InfoSocket => _infoSocket;

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

        public IOutputSocket<AimState> StateSocket => _stateSocket;

        private void Awake () => ForceInit();

        private void OnDestroy () => UnsubcribeFromInfoObject();

        private void SubscribeToInfoObject (IInfo infoObject) => infoObject.NameSocket.ReadOnlyProperty.Subscribe(UpdateName).AddTo(_disposables);

        private void UnsubcribeFromInfoObject () => _disposables.Dispose();

        private void UpdateGraphicalState (AimState state) => _aimImage.color = state switch
        {
            AimState.Normal => _normalColor,
            AimState.Highlighted => _highlightedColor,
            _ => throw new NotImplementedException(),
        };

        private void UpdateName (string? name)
        {
            _objectName.text = name;
            _objectName.enabled = name is not null;
        }

        private void UpdateSubscriptions (IInfo? info)
        {
            UnsubcribeFromInfoObject();
            SubscribeToInfoObject(info ?? EmptyInfo.Instance);
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            if (_aimImage == null)
                _aimImage = GetComponent<Image>();

            if (_objectName == null)
                _objectName = GetComponent<TMP_Text>();

            _infoSocket = new();
            _infoSocket.ReadOnlyProperty.Subscribe(UpdateSubscriptions).AddTo(this);

            _stateSocket = new(AimState.Normal);
            _stateSocket.SubscribeTo(_infoSocket, static info => info == null ? AimState.Normal : AimState.Highlighted);
            _stateSocket.ReadOnlyProperty.Subscribe(UpdateGraphicalState).AddTo(this);

            _isInited = true;
        }
    }
}