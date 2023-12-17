#nullable enable

using System;

using Aqua.FlowSystem;
using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public sealed class SimpleWaterSourceViewModel : MonoBehaviour, ITickObject
    {
        private readonly Source<Water> _waterSource;

        [SerializeField]
        private Water _initValue;

        [SerializeField]
        private bool _isMulticonnection = false;

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

        private SimpleWaterSourceViewModel () : base() => _waterSource = new Source<Water>(new Water(), _isMulticonnection);

        public void Init (float startTime) => _waterSource.OutputSocket.TrySetValue(new Water(_volume, _ph, _temp));

        public void Tick (int tickNumber, float startTime, float tickTime)
        {
        }
    }
}