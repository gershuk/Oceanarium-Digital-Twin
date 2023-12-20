using Aqua.FlowSystem;
using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTankViewModel : MonoBehaviour, ITickObject
    {
        protected bool _isInited = false;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        protected double _maxVolume = 1;

        [SerializeField]
        protected SimpleWaterTankObjectView _objectView;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        protected float _ph = 17;

        [SerializeField]
        [Range(-272, 272)]
        protected float _temp = 20;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        protected float _volume = 1;

        [SerializeField]
        protected WaterInfoPanelView _waterInfoPanel;

        protected SimpleWaterTank _waterTank;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        public double _localTickTime = 1;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        public double _outVolume = 0;

        public IInputSocket<Water> InputColdWaterSocket => _waterTank.InputColdWaterSocket;
        public IInputSocket<Water> InputHotWaterSocket => _waterTank.InputHotWaterSocket;
        public IOutputSocket<double> MaxVolumeSocket => _waterTank.MaxVolumeSocket;
        public IOutputSocket<Water> OutputWaterSocket => _waterTank.OutputWaterSocket;
        public IOutputSocket<Water> StoredSubstanceSocket => _waterTank.StoredSubstanceSocket;

        public void Awake () => ForceInit();

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _waterTank = new SimpleWaterTank(new Water(_volume, _temp, _ph), _maxVolume, _outVolume, _localTickTime);

            if (_objectView == null)
                _objectView = GetComponent<SimpleWaterTankObjectView>();

            if (_objectView != null)
            {
                _objectView.ForceInit();
                _objectView.WaterSocket.SubscribeTo(StoredSubstanceSocket);
                _objectView.MaxVolumeSocket.SubscribeTo(MaxVolumeSocket);
            }

            if (_waterInfoPanel == null)
                _waterInfoPanel = GetComponent<WaterInfoPanelView>();

            if (_waterInfoPanel != null)
            {
                _waterInfoPanel.ForceInit();
                _waterInfoPanel.WaterSocket.SubscribeTo(StoredSubstanceSocket);
                _waterInfoPanel.MaxVolumeSocket.SubscribeTo(MaxVolumeSocket);
            }

            _isInited = true;
        }

        public void Init (float startTime) => _waterTank.Init(startTime);

        public void Tick (int tickNumber, float startTime, float tickTime) => _waterTank.Tick(tickNumber, startTime, tickTime);
    }
}