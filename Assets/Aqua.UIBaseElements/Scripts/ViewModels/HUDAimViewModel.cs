#nullable enable

using System;

using Aqua.SocketSystem;

using UnityEngine;
using UnityEngine.UI;

using UniRx;
using TMPro;
using Aqua.Items;

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
        private TMP_Text _objectName;

        [SerializeField]
        private Color _highlightedColor;

        private bool _isInited = false;

        [SerializeField]
        private Color _normalColor;

        private ConverterSocket<IInfo, AimState> _stateSocket;
        private UniversalSocket<IInfo, IInfo> _infoSocket;

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
        public IInputSocket<IInfo> InfoSocket => _infoSocket;

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

            if (_objectName == null)
                _objectName = GetComponent<TMP_Text>();

            _infoSocket = new();
            _infoSocket.ReadOnlyProperty.Subscribe(UpdateSubscriptions).AddTo(this);

            _stateSocket = new(AimState.Normal);
            _stateSocket.SubscribeTo(_infoSocket, static info => info == null ? AimState.Normal : AimState.Highlighted);
            _stateSocket.ReadOnlyProperty.Subscribe(UpdateGraphicalState).AddTo(this);

            _isInited = true;
        }

        private void UpdateName (string? name)
        {
            _objectName.text = name;
            _objectName.enabled = name is null ? false : true;
        }

        private void UpdateSubscriptions (IInfo? info)
        {
            UnsubcribeFromInfoObject();
            SubscribeToInfoObject(info ?? EmptyInfo.Instance);
        }

        private void SubscribeToInfoObject (IInfo infoObject)
        {
            infoObject.NameSocket.ReadOnlyProperty.Subscribe(UpdateName).AddTo(_disposables);
        }

        private void UnsubcribeFromInfoObject ()
        {
            _disposables.Dispose();
        }

        private void OnDestroy ()
        {
            UnsubcribeFromInfoObject();
        }
    }
}