#nullable enable

using Aqua.TanksSystem.ViewModels;

using UniRx;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public sealed class ValveObjectView : MonoBehaviour
    {
        [SerializeField]
        private Transform _base;

        [SerializeField]
        private float _baseDelta = 20f;

        [SerializeField]
        private Vector3 _endAngle;

        private float _lastActionTime = 0f;

        [SerializeField]
        private Vector3 _startAngle;

        [SerializeField]
        private Transform _handle;

        [SerializeField]
        private ValveViewModel _viewmodel;

        private ReactiveCommand CloseCommand { get; } = new();
        private ReactiveCommand OpenCommand { get; } = new();

        private void RotateValve (float value) =>
            _handle.localRotation = Quaternion.Lerp(Quaternion.Euler(_startAngle),
                                              Quaternion.Euler(_endAngle),
                                              value);

        private void Start ()
        {
            if (_viewmodel == null)
                _viewmodel = GetComponent<ValveViewModel>();
            _viewmodel.Output.ReadOnlyProperty.Subscribe(RotateValve).AddTo(this);
            OpenCommand.Subscribe((u) => _viewmodel.Open(_baseDelta, Time.deltaTime));
            CloseCommand.Subscribe((u) => _viewmodel.Close(-_baseDelta, Time.deltaTime));
        }

        #region Rotation methods

        private bool TryExecuteCommand ()
        {
            var cond = Time.time - _lastActionTime - Time.deltaTime >= float.Epsilon;
            if (cond)
                _lastActionTime = Time.time;
            return cond;
        }

        [ContextMenu(nameof(Close))]
        public void Close ()
        {
            if (TryExecuteCommand())
                CloseCommand.Execute();
        }

        [ContextMenu(nameof(Open))]
        public void Open ()
        {
            if (TryExecuteCommand())
                OpenCommand.Execute();
        }

        #endregion Rotation methods
    }
}