using Aqua.FlowSystem;
using Aqua.SocketSystem;
using UniRx;

using TMPro;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public sealed class WaterInfoPanelView : MonoBehaviour
    {
        private bool _isInited = false;

        [SerializeField]
        private TMP_Text _volumeText;

        [SerializeField]
        private TMP_Text _temperatureText;

        [SerializeField]
        private TMP_Text _phText;

        private IUniversalSocket<double, double> _maxVolumeSocket;

        public IInputSocket<double> MaxVolumeSocket => _maxVolumeSocket;

        private IUniversalSocket<Water, Water> _waterSocket;

        public IInputSocket<Water> WaterSocket => _waterSocket;

        private CombiningSocket<Water, double, (Water, double)> _combiningSocket;

        public void ForceInit ()
        {
            if (_isInited) 
                return;

            _maxVolumeSocket = new UniversalSocket<double, double>();
            _waterSocket = new UniversalSocket<Water, Water>();
            _combiningSocket = new(combineFunction: static (w, l) => (w, l));
            _combiningSocket.SubscribeTo(_waterSocket);
            _combiningSocket.SubscribeTo(_maxVolumeSocket);
            _combiningSocket.ReadOnlyProperty.Subscribe(UpdateData);

            _isInited = true;
        }

        private void UpdateData ((Water water, double maxVolume) data)
        {
            _volumeText.text = $"{data.water.Volume / data.maxVolume * 100 :00.00}%";
            _temperatureText.text = $"{(data.water.Volume > 0 ? data.water.Temperature : 0): 00.00}°";
            _phText.text = $"{(data.water.Volume > 0 ? data.water.PH : 0): 00.00}";
        }

        private void Awake ()
        {
            ForceInit();
        }
    }
}
