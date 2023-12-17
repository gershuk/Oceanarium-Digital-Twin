using System;
using System.Collections.Generic;

using Aqua.FlowSystem;
using Aqua.SocketSystem;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTank : SimpleTank<Water>
    {
        protected readonly IUniversalSocket<Water, Water> _inputHotWaterSocket = new UniversalSocket<Water, Water>();
        protected readonly IUniversalSocket<Water, Water> _inputColdWaterSocket = new UniversalSocket<Water, Water>();
        protected readonly IUniversalSocket<Water, Water> _outputSocket = new UniversalSocket<Water, Water>();


        public float OutVolume { get; protected set; }

        public IInputSocket<Water> InputHotWaterSocket => _inputHotWaterSocket;
        public IInputSocket<Water> InputColdWaterSocket => _inputColdWaterSocket;
        public IOutputSocket<Water> OutputWaterSocket => _outputSocket;

        public SimpleWaterTank (double maxVolume = 1) : base(maxVolume)
        {
        }

        public SimpleWaterTank (Water waterData, double maxVolume = 1) : base(waterData, maxVolume)
        {
        }

        public override void Init (float startTime) => base.Init(startTime);

        public override void Tick (int tickNumber, float startTime, float tickTime)
        {
            base.Tick(tickNumber, startTime, tickTime);

            StoredValue = StoredValue.Combine(_inputHotWaterSocket.GetValue())
                                     .Combine(_inputColdWaterSocket.GetValue());

            var remCoef = Math.Max(StoredValue.Volume - OutVolume, 0) / StoredValue.Volume;
            var sep = StoredValue.Separate(remCoef, 1 - remCoef);

            StoredValue = sep[0];
            _outputSocket.TrySetValue(sep[1]);
        }
    }
}