using System.Collections;
using System.Collections.Generic;

using Aqua.FlowSystem;
using Aqua.SocketSystem;
using Aqua.TanksSystem.ViewModels;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTubeWithValveViewModel : SimpleWaterTubeViewModel
    {
        protected bool _isInited = false;

        [SerializeField]
        protected ValveViewModel _valveViewModel;

        protected CombiningSocket<Water, float, Water> _combiningSocket;

        public ValveViewModel Valve => _valveViewModel;

        public override IOutputSocket<Water> OutputSocket => _combiningSocket;

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

        protected void Awake ()
        {
            ForceInit();
        }
    }
}
