using System.Collections;
using System.Collections.Generic;

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTank : SimpleTank<WaterData>
    {
        protected readonly UniversalSocket<WaterData, WaterData> _inputSocket = new();

        public SimpleWaterTank () : base() { }

        public SimpleWaterTank (WaterData waterData) : base(waterData) { }

        public IInputSocket<WaterData> InputSocket => _inputSocket;

        public override void Init (float startTime)
        {
            base.Init(startTime);
        }

        public override void Tick (int tickNumber, float startTime, float tickTime)
        {
            base.Tick(tickNumber, startTime, tickTime);
            _data.Value += _inputSocket.GetValue();
        }
    }
}
