using Aqua.TanksSystem.ViewModels;

using UnityEngine;

namespace Aqua.TanksSystem
{
    [RequireComponent(typeof(TickSystem))]
    public class ValveTestScene : MonoBehaviour
    {
        [SerializeField]
        private TickSystem _tickSystem;

        [SerializeField]
        private ValveViewModel _valve;

        [SerializeField]
        private SimpleWaterSourceViewModel _waterSource;

        [SerializeField]
        private SimpleWaterTankViewModel _waterTank;

        [SerializeField]
        private SimpleWaterTubeViewModel _waterTube;

        private void Start ()
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