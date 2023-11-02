using System;

using Aqua.DataReader;

namespace Aqua.TanksSystem
{
    public class HistorySource<T> : Source<T>, ITickObject
    {
        protected float _currentTime;
        protected int _dataIndex;
        protected DataArray<T> _source;

        protected void UpdateDataIndex ()
        {
            var index = Array.BinarySearch(_source.Data,
                                           new Data<T>(Convert.ToDateTime(_currentTime), default),
                                           ComparerDataByTime<T>.Instance);
            _dataIndex = index >= 0 ? index : ~index - 1;
        }

        public void Init (float startTime)
        {
            _currentTime = startTime;
            _dataIndex = 0;
            UpdateDataIndex();
        }

        public void Tick (int tickNumber, float startTime, float tickTime)
        {
            _currentTime = startTime + (tickTime * tickNumber);
            UpdateDataIndex();
            _socket.TrySetValue(_source.Data[_dataIndex].Value);
        }
    }
}