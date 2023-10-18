using UnityEngine;

namespace Aqua.TubesCreater.Curves
{
    public class CubicPoly3D
    {
        private Vector3 _c0;
        private Vector3 _c1;
        private Vector3 _c2;
        private Vector3 _c3;

        /*
         * Compute coefficients for a cubic polynomial
         *   p(s) = c3*s^3 + c2*s^2 + c1*s + c0
         * such that
         *   p(0) = x0, p(1) = x1
         *  and
         *   p'(0) = t0, p'(1) = t1.
         */

        public CubicPoly3D (Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float tension = 0.5f)
        {
            var t0 = tension * (v2 - v0);
            var t1 = tension * (v3 - v1);

            _c0 = v1;
            _c1 = t0;
            _c2 = (-3f * v1) + (3f * v2) - (2f * t0) - t1;
            _c3 = (2f * v1) - (2f * v2) + t0 + t1;
        }

        public Vector3 Calculate (float t) =>
            (_c3 * Mathf.Pow(t, 3)) + (_c2 * Mathf.Pow(t, 2)) + (_c1 * t) + _c0;
    }
}