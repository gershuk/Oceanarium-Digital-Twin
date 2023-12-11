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

        public Water (double volume = 0, double ph = 0, double temperature = 0)
        {
            _volume = volume;
            _ph = ph;
            _temperature = temperature;
        }

        public double Volume => _volume;

        public double PH => _ph;

        public double Temperature => _temperature;

        public Water[] Separate (params double[] coefficients)
        {
            var volumes = new Water[coefficients.Length];

            for (var i = 0; i < coefficients.Length; ++i)
                volumes[i] = new Water(_volume * coefficients[i], _ph, _temperature);

            return volumes;
        }

        public Water Combine (params Water[] substances) // Need correct rule for combining temp and PH
        {
            var substance = new Water(_volume, _ph, _temperature);
            for (var i = 0; i < substances.Length; ++i)
                substance._volume += substances[i].Volume;

            return substance;
        }

        public bool IsVolumeApproximatelyEqual (double value) =>
            Math.Abs(_volume - value) < Utils.Eps;

        public bool IsVolumeApproximatelyLess (double value) =>
            value - _volume > Utils.Eps;
    }
}
