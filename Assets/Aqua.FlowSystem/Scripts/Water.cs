using System;

using UnityEngine;

namespace Aqua.FlowSystem
{
    [Serializable]
    public struct Water : ISubstance, ISubstanceOperations<Water>
    {
        [SerializeField]
        private double _volume;

        public Water (double volume)
        {
            _volume = volume;
        }

        public double Volume => _volume;

        public Water[] Separate (params double[] coefficients)
        {
            var volumes = new Water[coefficients.Length];

            for (var i = 0; i < coefficients.Length; ++i)
                volumes[i] = new Water(_volume * coefficients[i]);

            return volumes;
        }

        public Water Combine (params Water[] substances)
        {
            var substance = new Water(_volume);
            for (var i = 0; i < substances.Length; ++i)
                substance._volume += substances[i].Volume;

            return substance;
        }

        public bool IsVolumeApproximatelyEqual (double value, double eps) =>
            Math.Abs(_volume - value) < eps;

        public bool IsVolumeApproximatelyLess (double value, double eps) =>
            value - _volume > eps;
    }
}
