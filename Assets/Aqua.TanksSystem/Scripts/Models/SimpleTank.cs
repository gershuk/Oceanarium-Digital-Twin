using UnityEngine;

using Aqua.SocketSystem;
using UniRx;

namespace Aqua.TanksSystem
{
    public class SimpleTank<T> : ITickObject
    {
        protected ReactiveProperty<T> _data;

        protected IUniversalSocket<T, T> _universalSocket;

        public IOutputSocket<T> DataSocket => _universalSocket;

        public SimpleTank ()
        {
            _data = new();
            _universalSocket = new UniversalSocket<T, T>(_data);
        }

        public SimpleTank(T parameters)
        {
            _data = new ReactiveProperty<T>(parameters);
            _universalSocket = new UniversalSocket<T, T>(_data);
        }

        public virtual void Init (float startTime)
        {
           
        }

        public virtual void Tick (int tickNumber, float startTime, float tickTime)
        {
        }
    }
}
