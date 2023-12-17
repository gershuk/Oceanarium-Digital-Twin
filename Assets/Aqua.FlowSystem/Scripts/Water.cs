using System;

using UnityEngine;

namespace Aqua.FlowSystem
{
    [Serializable]
    public struct Water : ISubstance, ISubstanceOperations<Water>
    {
        [SerializeField]
        private double _volume;
        [SerializeField]
        private double _ph;
        [SerializeField]
        private double _temperature;

        public Water (double volume = 0,  double temperature = 0, double ph = 0) => 
            (_volume, _ph, _temperature) = (volume, ph, temperature);

        public double Volume
        {
            get => _volume;
            private set => _volume = value;
        }

        public double PH
        {
            get => _ph;
            private set => _ph = value;
        }

        public double Temperature
        {
            get => _temperature;
            private set => _temperature = value;
        }

        public Water[] Separate (params double[] coefficients)
        {
            var volumes = new Water[coefficients.Length];

            for (var i = 0; i < coefficients.Length; ++i)
                volumes[i] = new Water(Volume * coefficients[i], Temperature, PH * coefficients[i]);

            return volumes;
        }

        public Water Combine (params Water[] substances)
        {
            var newSubstance = new Water(Volume, Temperature, PH);

            for (var i = 0; i < substances.Length; ++i)
            {
                newSubstance.Temperature = (newSubstance.Temperature * newSubstance.Volume + 
                                           substances[i].Temperature * substances[i].Volume) / (newSubstance.Volume + substances[i].Volume);
                newSubstance.Volume += substances[i].Volume;
                newSubstance.PH += substances[i].PH;
            }

            return newSubstance;
        }

        public bool IsVolumeApproximatelyEqual (double value, double eps = double.Epsilon) => Math.Abs(Volume - value) < eps;

        public bool IsVolumeApproximatelyLess (double value, double eps = double.Epsilon) => Math.Abs(value - Volume) > eps;
    }
}
