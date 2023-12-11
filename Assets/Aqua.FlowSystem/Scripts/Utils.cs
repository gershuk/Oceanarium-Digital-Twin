using System;
using System.Collections.Generic;
using System.Linq;

namespace Aqua.FlowSystem
{
    public static class Utils
    {
        public static double Eps = 1e-3;

        public static void NormalizeCoefficients (IList<double> values)
        {
            var cefficiensSum = values.Sum();

            for (var i = 0; i < values.Count; ++i)
                values[i] /= cefficiensSum;
        }
    }
}
