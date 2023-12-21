#nullable enable

using System.Collections.Generic;

using Aqua.TickSystem;

using UnityEngine;

using System;

namespace Aqua.Scada
{
    public sealed class Visitor : MonoBehaviour, ITickObject
    {
        [SerializeField]
        private WayDispatcher? _initWay;

        private bool _isInited = false;

        private Transform _transform;

        public Transform Transform => _transform;

        [SerializeField]
        [Range(0f, 100f)]
        private float _speed = 1f;

        public WayDispatcher? WayDispatcher
        {
            get => _wayDispatcher;
            private set
            {
                _wayDispatcher = value;
                _segments?.Dispose();
                _segments = null;
            }
        }

        private IEnumerator<WaySegment>? _segments;
        private WayDispatcher? _wayDispatcher;

        private void Awake ()
        {
            ForceInit();
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _transform = transform;
            SetWayDispatcher(_initWay);

            _isInited = true;
        }

        public WaySegment CurrentSegment => _segments?.Current ?? throw new NullReferenceException(nameof(_segments));

        public void SetPositionAndSegment (WayDispatcher way, in WaySegment segment, Vector3 position)
        {
            SetWayDispatcher(way);

            if (_segments == null)
            {
                Debug.LogError("Segments == null");
                return;
            }

            _transform.position = position;

            while (CurrentSegment != segment)
            {
                GetNextSegment();
            }
        }

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public void Init (float startTime)
        {

        }

        public void SetWayDispatcher (WayDispatcher? way)
        {
            WayDispatcher = way;

            if (WayDispatcher != null)
            {
                _segments = WayDispatcher.GetEnumerator();
                _segments.MoveNext();
                _transform.position = CurrentSegment.FirstPoint;
                RotateToTarget();
            }
        }

        private void RotateToTarget ()
        {
            var dir = CurrentSegment.SecondPoint - _transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            _transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void GetNextSegment ()
        {
            if (_segments == null)
            {
                Debug.LogError("Segments == null");
                return;
            }

            if (WayDispatcher == null)
            {
                Debug.LogError("WayDispatcher == null");
                return;
            }

            if (!_segments.MoveNext())
            {
                SetWayDispatcher(WayDispatcher.NextWay());
            }

            RotateToTarget();
        }

        public void Tick (int tickNumber, float startTime, float tickTime)
        {
            if (_segments == null)
            {
                Debug.LogError("Segments == null");
                return;
            }

            if (WayDispatcher == null)
            {
                Debug.LogError("WayDispatcher == null");
                return;
            }

            if (CurrentSegment.IsEnd(_transform.position))
            {
                GetNextSegment();
            }

            var motion = Speed;

            while (motion > 0)
            {
                (_transform.position, motion) = CurrentSegment.GetClampedDisplacement(_transform.position, motion * tickTime);

                if (motion > 0)
                {
                    GetNextSegment();
                }
            }
        }
    }
}
