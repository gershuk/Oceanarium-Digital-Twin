#nullable enable

using System.Collections.Generic;

using Aqua.TickSystem;

using UnityEngine;
using System;

namespace Aqua.Scada
{
    public class VisitorCreater : MonoBehaviour
    {
        [SerializeField]
        private GameObject _visitorPrefab;

        private Visitor _defaultVisitor;

        [SerializeField]
        private ClockGenerator _clockGenerator;

        [SerializeField]
        private ClockController? _clockController;

        private bool _isInited = false;

        [SerializeField]
        private List<WayDispatcher> _startWays;

        [SerializeField]
        [Range(1, 100000)]
        private int _visitorsCount = 20;

        protected List<Visitor> _visitors;
        private bool _isClocksEnable;

        public List<WayDispatcher> StartWays 
        { 
            get => _startWays;
            set
            {
                _startWays = value;
                RespawnVisitors();
            }
        }

        private void Awake ()
        {
            ForceInit();
        }

        private void CreateDefaultVisitor ()
        {
            _defaultVisitor = new GameObject().AddComponent<Visitor>();
            _defaultVisitor.transform.parent = transform;
            _defaultVisitor.gameObject.name = "DefaultVisitor";
            _defaultVisitor.enabled = false;
        }

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _visitors = new();

            if (_clockGenerator == null)
                _clockGenerator = FindFirstObjectByType<ClockGenerator>();

            if (_clockController == null)
                _clockController = FindFirstObjectByType<ClockController>();

            CreateDefaultVisitor();

            _isInited = true;
        }

        protected bool IsClocksEnable
        {
            get => _isClocksEnable;
            set
            {
                _isClocksEnable = value;
                _clockGenerator.enabled = value;
                if (_clockController != null)
                    _clockController.enabled = value;
            }
        }

        protected int VisitorsCount { get => _visitorsCount; set => _visitorsCount = value; }

        [ContextMenu(nameof(RespawnVisitors))]
        public void RespawnVisitors ()
        {
            IsClocksEnable = false;
            foreach (var visitor in _visitors)
            {
                _clockGenerator.RemoveAll();
                Destroy(visitor.gameObject);
            }

            _visitors.Clear();

            _defaultVisitor.enabled = true;
            _defaultVisitor.Init(Time.deltaTime);

            foreach (var startWay in StartWays)
            {
                var startSegment = startWay.GetSegment(1);
                _defaultVisitor.SetPositionAndSegment(startWay, startSegment, startSegment.FirstPoint);

                var fullLength = startWay.Length;
                var nextWay = startWay.NextWay();
                while (nextWay != startWay)
                {
                    fullLength += nextWay.Length;
                    nextWay = nextWay.NextWay();
                }

                var speed = fullLength / VisitorsCount;

                for (var i = 0; i<VisitorsCount; ++i)
                {
                    _defaultVisitor.Speed = speed;

                    if (_defaultVisitor.WayDispatcher == null)
                        throw new NullReferenceException(nameof(_defaultVisitor.WayDispatcher)); 

                    SpawnVisitor(_defaultVisitor.WayDispatcher, _defaultVisitor.CurrentSegment, _defaultVisitor.Transform.position);
                    _defaultVisitor.Tick(i, 0, 1);
                }
            }

            _defaultVisitor.enabled = false;

            _clockGenerator.Init();
            IsClocksEnable = true;
        }

        private void SpawnVisitor (WayDispatcher way, in WaySegment segment, Vector3 position)
        {
            var visitor = Instantiate(_visitorPrefab).GetComponent<Visitor>();
            visitor.SetPositionAndSegment(way, segment, position);
            _clockGenerator.AddToEnd(visitor);
            _visitors.Add(visitor);
        }
    }
}
