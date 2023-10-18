using System;
using System.Collections.Generic;

using UnityEngine;

namespace Aqua.TubesCreater.Curves
{
    public struct FrenetFrame
    {
        public Vector3 Binormal { get; set; }
        public Vector3 Normal { get; set; }
        public Vector3 Position { get; set; }
        public float T { get; set; }
        public Vector3 Tangent { get; set; }

        public FrenetFrame (float t, Vector3 position, Vector3 tangent, Vector3 normal, Vector3 binormal) =>
            (T, Position, Tangent, Normal, Binormal) = (t, position, tangent, normal, binormal);
    }

    public abstract class AbstractCurve
    {
        protected float[] _arcLengths;
        protected Vector3[] _points;

        public IReadOnlyList<Vector3> Points => _points;

        public AbstractCurve (List<Vector3> points, int segmentsCount = 200)
        {
            _points = points.ToArray();
            UpdateArcLengths(segmentsCount);
        }

        protected Vector3 ChoîseNormal (Vector3 tangent)
        {
            var min = float.MaxValue;
            var normal = Vector3.zero;

            for (var i = 0; i < 3; ++i)
            {
                if (tangent[i] <= min)
                {
                    normal = Vector3.zero;
                    normal[i] = 1;
                }
            }

            return normal;
        }

        protected abstract Vector3 GetPointFromT (float t);

        protected virtual Vector3 GetTangentFromT (float t, float delta = 0.001f)
        {
            var t1 = Mathf.Max(0f, t - delta);
            var t2 = Mathf.Min(t + delta, 1f);

            return (GetPointFromT(t2) - GetPointFromT(t1)).normalized;
        }

        protected float GetUtoTmapping (float u)
        {
            var targetArcLength = u * _arcLengths[^1];

            var targetIndex = Array.BinarySearch(_arcLengths, u * _arcLengths[^1]);
            if (targetIndex < 0)
                targetIndex = Math.Max(0, ~targetIndex - 1);

            if (Mathf.Approximately(_arcLengths[targetIndex], targetArcLength))
                return 1f * targetIndex / (_arcLengths.Length - 1);

            var lengthBefore = _arcLengths[targetIndex];
            var lengthAfter = _arcLengths[targetIndex + 1];
            var segmentLength = lengthAfter - lengthBefore;
            var segmentFraction = (targetArcLength - lengthBefore) / segmentLength;

            return (targetIndex + segmentFraction) / (_arcLengths.Length - 1);
        }

        protected void UpdateArcLengths (int segmentsCount = 200)
        {
            if (segmentsCount < 0)
                throw new ArgumentOutOfRangeException(nameof(segmentsCount), $"Parameter must be >0");

            _arcLengths = new float[segmentsCount + 1];
            _arcLengths[0] = 0f;

            var lastPoint = GetPointFromT(0f);
            var length = 0f;
            var delta = 1f / segmentsCount;

            for (var p = 1; p <= segmentsCount; ++p)
            {
                var currentPoint = GetPointFromT(p * delta);
                length += Vector3.Distance(currentPoint, lastPoint);
                _arcLengths[p] = length;
                lastPoint = currentPoint;
            }
        }

        public FrenetFrame[] ComputeFrenetFrames (int segmentsCount = 100)
        {
            var delta = 1f / segmentsCount;
            var frames = new FrenetFrame[segmentsCount + 1];

            for (var i = 0; i <= segmentsCount; i++)
            {
                frames[i].T = i * delta;
                frames[i].Position = GetPointFromU(i * delta);
                frames[i].Tangent = GetTangentFromU(i * delta, delta).normalized;
            }

            var vec = Vector3.Cross(frames[0].Tangent, ChoîseNormal(frames[0].Tangent)).normalized;
            frames[0].Normal = Vector3.Cross(frames[0].Tangent, vec);
            frames[0].Binormal = Vector3.Cross(frames[0].Tangent, frames[0].Normal);

            for (var i = 1; i <= segmentsCount; i++)
            {
                frames[i].Normal = frames[i - 1].Normal;
                frames[i].Binormal = frames[i - 1].Binormal;

                var axis = Vector3.Cross(frames[i - 1].Tangent, frames[i].Tangent);
                if (axis.magnitude > float.Epsilon)
                {
                    var dot = Vector3.Dot(frames[i - 1].Tangent, frames[i].Tangent);
                    var theta = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f));
                    frames[i].Normal = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, axis.normalized) * frames[i].Normal;
                }

                frames[i].Binormal = Vector3.Cross(frames[i].Tangent, frames[i].Normal).normalized;
            }

            return frames;
        }

        public Vector3 GetPointFromU (float u) => GetPointFromT(GetUtoTmapping(u));

        public Vector3 GetTangentFromU (float u, float delta = 0.001f) => GetTangentFromT(GetUtoTmapping(u), delta);
    }
}