#nullable enable

using System.Collections.Generic;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public interface ITickObject
    {
        public void Init (float startTime);

        public void Tick (int tickNumber, float startTime, float tickTime);
    }

    public sealed class TickSystem : MonoBehaviour
    {
        [SerializeField]
        private float _startTime = 0;

        private int _tickNumber = 0;

        [SerializeField]
        private LinkedList<ITickObject> _tickObjectQueue = new();

        [SerializeField]
        private float _tickTime;

        private void Start ()
        {
            foreach (var tickObject in _tickObjectQueue)
            {
                tickObject.Init(_startTime);
            }
        }

        private void Update ()
        {
            foreach (var tickObject in _tickObjectQueue)
            {
                tickObject.Tick(_tickNumber, _startTime, _tickTime);
            }

            _tickNumber++;
        }

        public void AddToEnd (ITickObject tickObject) => _tickObjectQueue.AddLast(tickObject);

        public void Remove (ITickObject tickObject) => _tickObjectQueue.Remove(tickObject);
    }
}