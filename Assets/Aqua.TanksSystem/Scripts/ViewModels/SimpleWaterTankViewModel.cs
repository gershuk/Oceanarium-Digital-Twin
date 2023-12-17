using Aqua.FlowSystem;
using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTankViewModel : MonoBehaviour, ITickObject
    {
        private bool _isInited = false;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        private float _ph = 17;

        [SerializeField]
        [Range(-272, 272)]
        private float _temp = 20;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        private float _volume = 1;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        private double _maxVolume = 1;

        protected SimpleWaterTank _waterTank;

        [SerializeField]
        protected SimpleWaterTankObjectView _objectView;

        public IOutputSocket<Water> StoredSubstanceSocket => _waterTank.StoredSubstanceSocket;
        public IInputSocket<Water> InputHotWaterSocket => _waterTank.InputHotWaterSocket;
        public IInputSocket<Water> InputColdWaterSocket => _waterTank.InputColdWaterSocket;
        public IOutputSocket<Water> OutputWaterSocket => _waterTank.OutputWaterSocket;
        public IOutputSocket<double> MaxVolumeSocket => _waterTank.MaxVolumeSocket;

        public void ForceInit ()
        {
            if (_isInited) 
                return;

            _waterTank = new SimpleWaterTank(new Water(_volume, _temp, _ph), _maxVolume);

            if (_objectView == null)
                _objectView = GetComponent<SimpleWaterTankObjectView>();

            _objectView.ForceInit();
            _objectView.WaterSocket.SubscribeTo(StoredSubstanceSocket);
            _objectView.MaxVolumeSocket.SubscribeTo(MaxVolumeSocket);

            _isInited = true;
        }

        public void Awake ()
        {
            ForceInit();
        }

        public void Init (float startTime) => _waterTank.Init(startTime);

        public void Tick (int tickNumber, float startTime, float tickTime) => _waterTank.Tick(tickNumber, startTime, tickTime);
    }
}