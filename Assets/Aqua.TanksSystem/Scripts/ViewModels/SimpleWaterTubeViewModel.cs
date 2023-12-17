using Aqua.FlowSystem;
using Aqua.SocketSystem;
using Aqua.TanksSystem.Models;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTubeViewModel : MonoBehaviour
    {
        private readonly OneWayTubeModel<Water, Water> _tubeModel = new();

        public IInputSocket<Water> InputSocket => _tubeModel.InSocket;
        public IOutputSocket<Water> OutputSocket => _tubeModel.OutSocket;
    }
}