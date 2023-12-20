using Aqua.FlowSystem;
using Aqua.SocketSystem;
using Aqua.TanksSystem.ViewModels;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTubeWithValveViewModel : SimpleWaterTubeViewModel
    {
        protected CombiningSocket<Water, float, Water> _combiningSocket;
        protected bool _isInited = false;

        [SerializeField]
        protected ValveViewModel _valveViewModel;

        public override IOutputSocket<Water> OutputSocket => _combiningSocket;
        public ValveViewModel Valve => _valveViewModel;

        protected void Awake () => ForceInit();

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _valveViewModel.ForceInit();

            _combiningSocket = new(combineFunction: static (w, v) => w.Separate(v)[0]);
            _combiningSocket.SubscribeTo(_tubeModel.OutSocket);
            _combiningSocket.SubscribeTo(_valveViewModel.Output);

            _isInited = true;
        }
    }
}