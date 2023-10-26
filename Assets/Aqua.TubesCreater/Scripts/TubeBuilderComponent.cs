using System.Collections.Generic;

using UnityEngine;

using System;

using Aqua.TubesCreater.Curves;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Aqua.TubesCreater.Builders
{
    public enum CurveType
    {
        CatmullRom
    };

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class TubeBuilderComponent : MonoBehaviour
    {
        #region Gizmo Params

        [SerializeField]
        private bool _frame = false;

        [SerializeField]
        [Range(2, 10_000)]
        private int _gizmosCount = 20;

        [SerializeField]
        private float _gizmoSizeScale = 0.1f;

        [SerializeField]
        private bool _point = true;

        [SerializeField]
        private bool _tangent = true;

        [SerializeField]
        private bool _text = true;

        #endregion Gizmo Params

        #region Curve Params

        [SerializeField]
        private int _radialSegments = 6;

        [SerializeField]
        private float _radius = 0.1f;

        [SerializeField]
        private int _segmentsCount = 20;

        [SerializeField]
        private CurveType _type = CurveType.CatmullRom;

        #endregion Curve Params

        private AbstractCurve _curve;
        private MeshFilter _filter;

        #region Cache Parameters

        private bool _isFramesDirty = true;
        private int _lastGizmosCount = 0;

        #endregion Cache Parameters

        [SerializeField]
        private List<Vector3> _points;

        public FrenetFrame[] Frames { get; private set; }

        public float GizmoSize => _gizmoSizeScale;
        public List<Vector3> Points => _points;

        private void Awake ()
        {
            _filter = GetComponent<MeshFilter>();

            if (_points == null || _points.Count == 0)
            {
                _points = new List<Vector3>()
                {
                    Vector3.zero,
                    Vector3.up,
                    Vector3.right
                };
            }
            if (_filter == null)
                _filter = GetComponent<MeshFilter>();

            RebuildCurve();
        }

        [ContextMenu("Build Tube")]
        private void Build () => _filter.sharedMesh = TubeMeshBuilder.Build(_curve, _segmentsCount, _radius, _radialSegments);

        #region Drawing Gizmo Methods

        private void DrawFrame (FrenetFrame frame, Vector3 position)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + (frame.Tangent * GizmoSize));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(position, position + (frame.Normal * GizmoSize));

            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, position + (frame.Binormal * GizmoSize));
        }

        private void DrawPoint (Vector3 position)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(position, GizmoSize);
        }

        private void DrawTangent (Vector3 tangent, Vector3 position)
        {
            var colorVector = (tangent + Vector3.one) * 0.5f;
            Gizmos.color = new Color(colorVector.x, colorVector.y, colorVector.z);
            Gizmos.DrawLine(position, position + (tangent * GizmoSize));
        }

        #endregion Drawing Gizmo Methods

        private void OnDrawGizmos ()
        {
            if (_curve == null)
                return;

            if (_isFramesDirty || _lastGizmosCount != _gizmosCount)
            {
                Frames = _curve.ComputeFrenetFrames(_gizmosCount);
                _lastGizmosCount = _gizmosCount;
                _isFramesDirty = false;
            }

            Gizmos.matrix = transform.localToWorldMatrix;

            for (var i = 0; i < Frames.Length - 1; i++)
            {
                if (_point)
                    DrawPoint(Frames[i].Position);

                if (_tangent)
                    DrawTangent(Frames[i].Tangent, Frames[i].Position);

                if (_frame)
                    DrawFrame(Frames[i], Frames[i].Position);

#if UNITY_EDITOR
                if (_text)
                {
                    Handles.matrix = transform.localToWorldMatrix;
                    Handles.Label(Frames[i].Position, i.ToString());
                }
#endif
            }
        }

        private void OnEnable ()
        {
            if (_points == null || _points.Count == 0)
            {
                _points = new List<Vector3>()
                {
                    Vector3.zero,
                    Vector3.up,
                    Vector3.right
                };
            }
            if (_filter == null)
                _filter = GetComponent<MeshFilter>();

            RebuildCurve();
        }

        public void RebuildCurve ()
        {
            _curve = _type switch
            {
                CurveType.CatmullRom => new CatmullRomCurve(_points),
                _ => throw new NotImplementedException(),
            };
            _isFramesDirty = true;
        }
    }
}