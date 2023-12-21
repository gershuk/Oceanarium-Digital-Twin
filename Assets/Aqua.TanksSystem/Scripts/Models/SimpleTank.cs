using Aqua.FlowSystem;
using Aqua.SocketSystem;
using Aqua.TickSystem;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleTank<T> : ITickObject where T : ISubstance
    {
        protected MulticonnectionSocket<bool, bool> _isOverflowSocket;

        protected MulticonnectionSocket<double, double> _maxVolumeSocket;

        protected ConverterSocket<T, bool> _overflowCheckerSocket;

        protected MulticonnectionSocket<T, T> _storedSubstanceSocket;

        public IOutputSocket<bool> IsOverflowSocket => _isOverflowSocket;

        public double MaxVolume
        {
            get => _maxVolumeSocket.GetValue();
            protected set => _maxVolumeSocket.TrySetValue(value);
        }

        public IOutputSocket<double> MaxVolumeSocket => _maxVolumeSocket;
        public IOutputSocket<T> StoredSubstanceSocket => _storedSubstanceSocket;

        public T StoredValue
        {
            get => _storedSubstanceSocket.GetValue();
            protected set
            {
                if (!_storedSubstanceSocket.TrySetValue(value))
                    Debug.LogError("Can't set value when socket has input connection");
            }
        }

        public SimpleTank (double maxVolume = 1)
        {
            _storedSubstanceSocket = new MulticonnectionSocket<T, T>();
            _maxVolumeSocket = new(maxVolume);
            SetUpOverflowSockets();
        }

        public SimpleTank (T parameters, double maxVolume = 1)
        {
            _storedSubstanceSocket = new MulticonnectionSocket<T, T>(parameters);
            _maxVolumeSocket = new(maxVolume);
            SetUpOverflowSockets();
        }

        protected void SetUpOverflowSockets ()
        {
            _overflowCheckerSocket = new();
            _isOverflowSocket = new();
            _overflowCheckerSocket.SubscribeTo(_storedSubstanceSocket, w => w.Volume > MaxVolume);
            _isOverflowSocket.SubscribeTo(_overflowCheckerSocket);
        }

        public virtual void Init (float startTime)
        {
        }

        public virtual void Tick (int tickNumber, float startTime, float tickTime)
        {
        }
    }
}