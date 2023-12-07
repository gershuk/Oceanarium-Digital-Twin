using System;
using System.Collections.Generic;

using UnityEngine;

namespace Aqua.FlowSystem
{
    public class WaterPump : MonoBehaviour, IVolumeContainer<Water>
    {
        [SerializeField]
        private double _waterPerSecond;
        [SerializeField]
        private WaterSocket _socket;

        public double MaxVolume => 0;

        public Water StoredSubstance => new(0);

        public bool IsFull => true;

        public void AddSubstance (Water substance) => throw new InvalidOperationException();

        public IReadOnlyCollection<IFlowSocket<Water>> Sockets => new IFlowSocket<Water>[] { _socket };

        public void PassiveFlow () => throw new InvalidOperationException();
    }
}
