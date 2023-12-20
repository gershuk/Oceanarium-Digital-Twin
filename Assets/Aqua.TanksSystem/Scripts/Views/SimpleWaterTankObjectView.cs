#nullable enable

using Aqua.FlowSystem;
using Aqua.SocketSystem;

using UniRx;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTankObjectView : MonoBehaviour
    {
        private bool _isInited = false;

        [SerializeField]
        protected bool _autoSetCustomStartScale = false;

        protected CombiningSocket<Water, double, (Water, double)> _combiningSocket;

        [SerializeField]
        protected Vector3 _customStartScale;

        [SerializeField]
        protected Transform _endPosition;

        protected IUniversalSocket<double, double> _maxVolumeSocket;

        [SerializeField]
        protected AnimationCurve _scale;

        [SerializeField]
        protected Transform _startPosition;

        [SerializeField]
        protected Transform _water;

        protected IUniversalSocket<Water, Water> _waterSocket;
        public IInputSocket<double> MaxVolumeSocket => _maxVolumeSocket;

        public IInputSocket<Water> WaterSocket => _waterSocket;

        protected void SetLevel (double value)
        {
            if (!double.IsFinite(value))
            {
                Debug.LogWarning("Value is not valide. Can't update water level.");
                return;
            }
            _water.position = Vector3.Lerp(_startPosition.position, _endPosition.position, (float) value);
            _water.localScale = _customStartScale * _scale.Evaluate((float) value);
        }

        protected void Start () => ForceInit();

        protected void UpdateWaterLevel ((Water water, double maxLevel) data) => SetLevel(data.water.Volume / data.maxLevel);

        public void ForceInit ()
        {
            if (_isInited)
                return;

            if (_autoSetCustomStartScale)
                _customStartScale = _water.localScale;

            _waterSocket = new UniversalSocket<Water, Water>();
            _maxVolumeSocket = new UniversalSocket<double, double>();
            _combiningSocket = new CombiningSocket<Water, double, (Water, double)>(combineFunction: static (w, l) => (w, l));
            _combiningSocket.SubscribeTo(_waterSocket);
            _combiningSocket.SubscribeTo(_maxVolumeSocket);
            _combiningSocket.ReadOnlyProperty.Subscribe(UpdateWaterLevel);

            _isInited = true;
        }
    }
}