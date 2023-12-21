#nullable enable

using Aqua.SocketSystem;

using UnityEngine;
using UnityEngine.UI;

using UniRx;

namespace Aqua.UIBaseElements
{
    public sealed class SimpleToggleViewModel : MonoBehaviour
    {
        private bool _isInited = false;

        [SerializeField]
        private Toggle _toggle;

        private MulticonnectionSocket<bool, bool> _stateSocket;

        private ConverterSocket<bool, int> _converterSocket;

        private MulticonnectionSocket<int, int> _numberSocket;

        public IOutputSocket<bool> StateSocket => _stateSocket;
        public IOutputSocket<int> NumberSocket => _numberSocket;

        public bool State => StateSocket.GetValue();

        public void ForceInit ()
        {
            if (_isInited)
                return;

            if (_toggle == null)
                _toggle = GetComponent<Toggle>();

            _stateSocket = new(_toggle.isOn);

            _converterSocket = new();
            _converterSocket.SubscribeTo(_stateSocket, static b => b ? 1 : 0);

            _numberSocket = new();
            _numberSocket.SubscribeTo(_converterSocket);

            _toggle.OnValueChangedAsObservable().Subscribe((v)=>_stateSocket.TrySetValue(v));

            _isInited = true;
        }

        private void Awake ()
        {
            ForceInit();
        }
    }
}
