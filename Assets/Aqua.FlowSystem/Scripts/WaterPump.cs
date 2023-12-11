using System;
using System.Collections.Generic;

using UnityEngine;

namespace Aqua.FlowSystem
{
    public class WaterPump : MonoBehaviour, IVolumeContainer<Water>
    {
        [SerializeField]
        [Range(0f, 1000f)]
        private double _litersPerMinute;
        [SerializeField]
        private WaterSocket _socket;

        private void FixedUpdate ()
        {
            if (_litersPerMinute == 0)
                return;

            var flow = _litersPerMinute / 60 / Time.fixedDeltaTime;

            var remains = _socket.ActivePush(new Water(flow));
        }

        public double MaxVolume => 0;

        public double FreeVolume => 0;

        public Water StoredSubstance => default;

        public bool IsFull => true;

        public Water AddSubstance (Water substance) => throw new InvalidOperationException();

        public IReadOnlyCollection<IFlowSocket<Water>> Sockets => new IFlowSocket<Water>[] { _socket };
    }
}
