using System.Collections.Generic;

using UnityEngine;

namespace Aqua.TubesCreater.Curves
{
    public class CatmullRomCurve : AbstractCurve
    {
        public CatmullRomCurve (List<Vector3> points, int segmentsCount = 200) : base(points, segmentsCount)
        {
        }

        protected override Vector3 GetPointFromT (float t)
        {
            var l = _points.Length;

            var point = (l - 1) * t;
            var intPoint = Mathf.FloorToInt(point);
            var weight = point - intPoint;

            if (weight == 0 && intPoint == l - 1)
            {
                intPoint = l - 2;
                weight = 1;
            }

            Vector3 tmp, p0, p1, p2, p3; // 4 _points
            if (intPoint > 0)
            {
                p0 = _points[(intPoint - 1) % l];
            }
            else
            {
                // extrapolate first point
                tmp = _points[0] - _points[1] + _points[0];
                p0 = tmp;
            }

            p1 = _points[intPoint % l];
            p2 = _points[(intPoint + 1) % l];

            if (intPoint + 2 < l)
            {
                p3 = _points[(intPoint + 2) % l];
            }
            else
            {
                // extrapolate last point
                tmp = _points[l - 1] - _points[l - 2] + _points[l - 1];
                p3 = tmp;
            }

            var poly = new CubicPoly3D(p0, p1, p2, p3);
            return poly.Calculate(weight);
        }
    }
}