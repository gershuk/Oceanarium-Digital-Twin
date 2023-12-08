using System;
using System.Collections.Generic;

using UnityEngine;

namespace Aqua.FlowSystem
{
    public class WaterDrain : MonoBehaviour, IVolumeContainer<Water>
    {
        [SerializeField]
        private double _waterFlowDisplay;
        [SerializeField]
        private WaterSocket _socket;

        public double MaxVolume => double.MaxValue;

        public double FreeVolume => double.MaxValue;

        public Water StoredSubstance => new(0);

        public bool IsFull => false;

        public void AddSubstance (Water substance) => _waterFlowDisplay = substance.Volume;

        public IReadOnlyCollection<IFlowSocket<Water>> Sockets => new IFlowSocket<Water>[] { _socket };
    }
}
