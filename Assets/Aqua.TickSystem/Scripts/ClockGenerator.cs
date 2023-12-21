#nullable enable

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Aqua.TickSystem
{
    public interface ITickObject
    {
        public void Init (float startTime);

        public void Tick (int tickNumber, float startTime, float tickTime);
    }

    public interface ITickSystem
    {
        void AddToEnd (ITickObject tickObject);

        void Init ();

        void Remove (ITickObject tickObject);

        void Tick ();
    }

    public sealed class ClockGenerator : MonoBehaviour, ITickSystem
    {
        private float _lastTickTime = 0;

        [SerializeField]
        private float _startTime = 0;

        private int _tickNumber = 0;

        [SerializeField]
        private LinkedList<ITickObject> _tickObjectQueue = new();

        [SerializeField]
        private float _tickTime;

        public void AddToEnd (ITickObject tickObject) => _tickObjectQueue.AddLast(tickObject);

        public void Init ()
        {
            _tickNumber = 0;
            _lastTickTime = Time.time - _tickTime;
            foreach (var tickObject in _tickObjectQueue)
            {
                tickObject.Init(_startTime);
            }
        }

        public void Remove (ITickObject tickObject) => _tickObjectQueue.Remove(tickObject);
        public void RemoveAll () => _tickObjectQueue.Clear();

        public void Tick ()
        {
            if (_lastTickTime + _tickTime <= Time.time)
            {
                foreach (var tickObject in _tickObjectQueue)
                {
                    tickObject.Tick(_tickNumber, _startTime, _tickTime);
                }

                _tickNumber++;
                _lastTickTime = Time.time;
            }
        }
    }
}