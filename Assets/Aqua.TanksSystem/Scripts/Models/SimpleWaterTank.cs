using System;
using UnityEngine;

using Aqua.FlowSystem;
using Aqua.SocketSystem;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTank : SimpleTank<Water>
    {
        protected readonly IUniversalSocket<Water, Water> _inputHotWaterSocket = new UniversalSocket<Water, Water>();
        protected readonly IUniversalSocket<Water, Water> _inputColdWaterSocket = new UniversalSocket<Water, Water>();
        protected readonly IUniversalSocket<Water, Water> _outputSocket = new UniversalSocket<Water, Water>();

        public double OutVolume { get; protected set; }

        public double? LocalTickTime { get; protected set; }

        public double TickScale (double tickTime) => LocalTickTime.HasValue ? tickTime / LocalTickTime.Value : 1;

        public IInputSocket<Water> InputHotWaterSocket => _inputHotWaterSocket;
        public IInputSocket<Water> InputColdWaterSocket => _inputColdWaterSocket;
        public IOutputSocket<Water> OutputWaterSocket => _outputSocket;

        public SimpleWaterTank (double maxVolume = 1, double outVolume = 0, double? localTickTime = default) : base(maxVolume)
        {
            OutVolume = outVolume;
            LocalTickTime = localTickTime;
        }

        public SimpleWaterTank (Water waterData,
                                double maxVolume = 1,
                                double outVolume = 0,
                                double? localTickTime = default) : base(waterData, maxVolume)
        {
            OutVolume = outVolume;
            LocalTickTime = localTickTime;
        }

        public override void Init (float startTime) => base.Init(startTime);

        public override void Tick (int tickNumber, float startTime, float tickTime)
        {
            base.Tick(tickNumber, startTime, tickTime);

            var hotInput = _inputHotWaterSocket.GetValue().Separate(TickScale(tickTime));
            var coldInput = _inputColdWaterSocket.GetValue().Separate(TickScale(tickTime));

            StoredValue = StoredValue.Combine(hotInput).Combine(coldInput);
            
            var remCoef = Math.Max(StoredValue.Volume - OutVolume * TickScale(tickTime), 0) / StoredValue.Volume;
            var sep = StoredValue.Separate(remCoef, 1 - remCoef);

            StoredValue = sep[0];
            _outputSocket.TrySetValue(sep[1]);
        }
    }
}