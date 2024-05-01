using System;
using System.Collections;
using System.Collections.Generic;

using Aqua.SocketSystem;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;

namespace Aqua.Scada
{
    public readonly struct WaySegment
    {
        private readonly WayDispatcher _way;

        public readonly Vector3 FirstPoint;
        public readonly Vector3 SecondPoint;

        public float Length => Vector3.Distance(FirstPoint, SecondPoint);

        public Vector3 GetPosition (float percent) => Vector3.Lerp(FirstPoint, SecondPoint, percent);

        public Vector3 GetDisplacement (float speed) => (SecondPoint - FirstPoint).normalized * speed;

        public bool IsPointBelongToSegment (Vector3 position) =>
            Math.Abs(Vector3.Distance(position, FirstPoint) + Vector3.Distance(position, SecondPoint) - Length) < 1e-4
            || Vector3.Distance(position, FirstPoint) < 1e-4
            || Vector3.Distance(position, SecondPoint) < 1e-4;

        public (Vector3 newPosition, float speedReminder) GetClampedDisplacement (Vector3 position, float speed)
        {
            if (!IsPointBelongToSegment(position))
            {
                Debug.LogWarning($"Point {position} does not belong to segment {FirstPoint}-{SecondPoint}.");
            }

            var newPosition = position + GetDisplacement(speed);
            var newlength = Vector3.Distance(newPosition, FirstPoint);
            return (newlength > Length) ? (SecondPoint, newlength - Length) : (newPosition, 0);
        }

        public bool IsEnd (Vector3 position) => position == SecondPoint;
        public bool IsStart (Vector3 position) => position == FirstPoint;

        public WaySegment (Vector3 firstPoint, Vector3 secondPoint, WayDispatcher way) =>
            (FirstPoint, SecondPoint, _way) = (firstPoint, secondPoint, way);

        public static bool operator == (WaySegment s1, WaySegment s2)
        {
            return s1.Equals(s2);
        }

        public static bool operator != (WaySegment s1, WaySegment s2)
        {
            return !s1.Equals(s2);
        }
    }

    [ExecuteInEditMode]
    [Serializable]
    public class WayDispatcher : MonoBehaviour, IEnumerable<WaySegment>
    {
        [SerializeField]
        private bool _drawGUI = false;

        [SerializeField]
        private List<WayDispatcher> _nextWayes;

        private MulticonnectionSocket<int, int> _nextWayIndex;

        public WayDispatcher NextWay () => _nextWayes[_nextWayIndex.GetValue()];

        private bool _isInited = false;
        private Transform _transform;

        [SerializeField]
        private List<Vector3> _points;

        public IReadOnlyList<Vector3> Points => _points;
        public List<Vector3> PointsForEditor => _points;

        public float Length
        {
            get
            {
                var l = 0f;
                foreach (var segment in this)
                {
                    l += segment.Length;
                }
                return l;
            }
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _transform = transform;
            _nextWayIndex = new(0);



            _isInited = true;
        }

        private void Awake ()
        {
            ForceInit();
        }

        public WaySegment GetSegment (int i)
        {
            var fistPoint = _transform.TransformPoint(_points[i - 1]);
            var secondPoint = _transform.TransformPoint(_points[i]);
            return new(fistPoint, secondPoint, this);
        }

        public bool IsSegmentLast (int i) => i == _points.Count - 1;

        #region IEnumerable<Vector3>
        public IEnumerator<WaySegment> GetEnumerator ()
        {
            for (var i = 1; i < _points.Count; ++i)
            {
                yield return GetSegment(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
        #endregion

        void DrawLine (Vector2 pointA, Vector2 pointB, Color color, float width = 2.0f)
        {
            pointA.y = Screen.height - pointA.y;
            pointB.y = Screen.height - pointB.y;
            var lineTex = new Texture2D(1, 1);
            var matrixBackup = GUI.matrix;
            GUI.color = color;
            var angle = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x) * 180f / Mathf.PI;
            GUIUtility.RotateAroundPivot(angle, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, Vector2.Distance(pointA, pointB), width), lineTex);
            GUI.matrix = matrixBackup;
        }

        private void OnGUI ()
        {
            if (!_drawGUI)
                return;

            if (_points == null)
                return;

            foreach (var segment in this)
            {
                DrawLine(segment.FirstPoint, segment.SecondPoint, Color.green);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos ()
        {
            if (_points == null)
                return;

            Gizmos.color = Color.green;
            foreach (var segment in this)
            {
                Gizmos.DrawLine(segment.FirstPoint, segment.SecondPoint);
            }

            using (new Handles.DrawingScope(Color.red, transform.localToWorldMatrix))
            {
                for (var i = 0; i < _points.Count; ++i)
                {
                    Handles.Label(_points[i], i.ToString());
                }
            }
        }
#endif
    }
}