#nullable enable

using System;

using Aqua.FlowSystem;
using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public sealed class SimpleWaterSourceViewModel : MonoBehaviour, ITickObject
    {
        private ConverterSocket<Water, double> _maxVolumeConverter;

        private bool _isInited = false;

        [SerializeField]
        private WaterInfoPanelView _waterInfoPanel;

        private Source<Water> _waterSource;

        [SerializeField]
        private Water _initValue;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        private float _ph = 17;

        [SerializeField]
        [Range(-272, 272)]
        private float _temp = 20;

        [SerializeField]
        [Range(0.0f, 1e6f)]
        private float _volume = 1;

        public IOutputSocket<Water> OutputSocket => _waterSource.OutputSocket;

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _waterSource = new Source<Water>(new Water(_volume, _temp, _ph), true);
            _maxVolumeConverter = new();
            _maxVolumeConverter.SubscribeTo(_waterSource.OutputSocket, static w=>w.Volume);

            if (_waterInfoPanel == null)
                _waterInfoPanel = GetComponent<WaterInfoPanelView>();

            if (_waterInfoPanel != null)
            {
                _waterInfoPanel.ForceInit();
                _waterInfoPanel.WaterSocket.SubscribeTo(OutputSocket);
                _waterInfoPanel.MaxVolumeSocket.SubscribeTo(_maxVolumeConverter);
            }

            _isInited = true;
        }

        private void Awake ()
        {
            ForceInit();
        }

        public void Init (float startTime) => ForceInit();

        public void Tick (int tickNumber, float startTime, float tickTime)
        {
        }
    }
}