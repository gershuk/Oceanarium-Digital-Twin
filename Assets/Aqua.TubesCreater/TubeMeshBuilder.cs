using System.Collections.Generic;

using Aqua.TubesCreater.Curves;

using UnityEngine;

namespace Aqua.TubesCreater.Builders
{
    public static class TubeMeshBuilder
    {
        private const float PI2 = Mathf.PI * 2f;

        private static void GenerateSegment (AbstractCurve curve,
                                             FrenetFrame[] frames,
                                             int tubularSegments,
                                             float radius,
                                             int radialSegments,
                                             List<Vector3> vertices,
                                             List<Vector3> normals,
                                             List<Vector4> tangents,
                                             int i)
        {
            var u = 1f * i / tubularSegments;
            var position = curve.GetPointFromU(u);
            var fr = frames[i];

            for (var j = 0; j <= radialSegments; j++)
            {
                var v = 1f * j / radialSegments * PI2;
                var sin = Mathf.Sin(v);
                var cos = Mathf.Cos(v);

                var normal = ((cos * fr.Normal) + (sin * fr.Binormal)).normalized;
                vertices.Add(position + (radius * normal));
                normals.Add(normal);

                var tangent = fr.Tangent;
                tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 0f));
            }
        }

        public static Mesh Build (AbstractCurve curve, int tubularSegments, float radius, int radialSegments)
        {
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var tangents = new List<Vector4>();
            var uvs = new List<Vector2>();
            var indices = new List<int>();

            var frames = curve.ComputeFrenetFrames(tubularSegments);
            var linearDelta = 1f / tubularSegments;
            var angularDelta = 1f / radialSegments;

            for (var i = 0; i < tubularSegments; i++)
            {
                GenerateSegment(curve, frames, tubularSegments, radius, radialSegments, vertices, normals, tangents, i);
            }
            GenerateSegment(curve, frames, tubularSegments, radius, radialSegments, vertices, normals, tangents, tubularSegments);

            for (var i = 0; i <= tubularSegments; i++)
            {
                for (var j = 0; j <= radialSegments; j++)
                {
                    var u = angularDelta * j;
                    var v = linearDelta * i;
                    uvs.Add(new Vector2(u, v));
                }
            }

            for (var j = 1; j <= tubularSegments; j++)
            {
                for (var i = 1; i <= radialSegments; i++)
                {
                    var a = ((radialSegments + 1) * (j - 1)) + (i - 1);
                    var b = ((radialSegments + 1) * j) + (i - 1);
                    var c = ((radialSegments + 1) * j) + i;
                    var d = ((radialSegments + 1) * (j - 1)) + i;

                    // faces
                    indices.Add(a);
                    indices.Add(d);
                    indices.Add(b);
                    indices.Add(b);
                    indices.Add(d);
                    indices.Add(c);
                }
            }

            var mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                normals = normals.ToArray(),
                tangents = tangents.ToArray(),
                uv = uvs.ToArray()
            };
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
            return mesh;
        }
    }
}