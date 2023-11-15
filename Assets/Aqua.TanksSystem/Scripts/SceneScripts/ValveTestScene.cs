using System.Collections;
using System.Collections.Generic;

using Aqua.TanksSystem.ViewModels;

using UnityEngine;

namespace Aqua.TanksSystem
{
    [RequireComponent(typeof(TickSystem))]
    public class ValveTestScene : MonoBehaviour
    {
        [SerializeField]
        private SimpleWaterSourceViewModel _waterSource;

        [SerializeField]
        private ValveViewModel _valve;

        [SerializeField]
        private SimpleWaterTubeViewModel _waterTube;

        [SerializeField]
        private SimpleWaterTankViewModel _waterTank;

        [SerializeField]
        private TickSystem _tickSystem;

        void Start()
        {
            if (_tickSystem == null)
                _tickSystem = GetComponent<TickSystem>();

            _waterTank.InputSocket.SubscribeTo(_waterTube.OutputSocket);
            _waterTube.InputSocket.SubscribeTo(_waterSource.OutputSocket);

            _tickSystem.AddToEnd(_waterSource);
            _tickSystem.AddToEnd(_waterTank);
        }
    }
}
