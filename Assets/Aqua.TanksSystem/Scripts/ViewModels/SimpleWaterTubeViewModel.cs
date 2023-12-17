using Aqua.FlowSystem;
using Aqua.SocketSystem;
using Aqua.TanksSystem.Models;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTubeViewModel : MonoBehaviour
    {
        protected readonly OneWayTubeModel<Water, Water> _tubeModel = new();

        public virtual IInputSocket<Water> InputSocket => _tubeModel.InSocket;
        public virtual IOutputSocket<Water> OutputSocket => _tubeModel.OutSocket;
    }
}