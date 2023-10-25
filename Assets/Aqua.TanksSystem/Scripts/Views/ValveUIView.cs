using Aqua.TanksSystem.ViewModels;
using Aqua.UIBaseElements;

using UniRx;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public sealed class ValveUIView : FloatViewUI
    {
        [SerializeField]
        private float _baseDelta = 20f;

        [SerializeField]
        private ValveViewModel _viewmodel;

        // Update is called once per frame
        private void Update ()
        {
        }

        protected override void Start ()
        {
            base.Start();

            if (_viewmodel == null)
                _viewmodel = GetComponent<ValveViewModel>();

            Socket.SubscribeTo(_viewmodel.Output);
            IncreaseValueCommand.Subscribe((u) => _viewmodel.Open(_baseDelta, Time.deltaTime));
            DecreaseValueCommand.Subscribe((u) => _viewmodel.Close(-_baseDelta, Time.deltaTime));
            ResetValueCommand.Subscribe((u) => _viewmodel.ResetValue());
        }
    }
}