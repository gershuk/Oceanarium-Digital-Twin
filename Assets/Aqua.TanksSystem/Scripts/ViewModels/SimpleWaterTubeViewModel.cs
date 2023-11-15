using Aqua.SocketSystem;
using Aqua.TanksSystem.Models;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTubeViewModel : MonoBehaviour
    {
        private readonly OneWayTubeModel<WaterData, WaterData> _tubeModel = new();

        public IInputSocket<WaterData> InputSocket => _tubeModel.InSocket;
        public IOutputSocket<WaterData> OutputSocket => _tubeModel.OutSocket;
    }
}