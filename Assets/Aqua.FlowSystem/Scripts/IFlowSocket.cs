#nullable enable

using System;
using System.Collections.Generic;

namespace Aqua.FlowSystem
{
    public interface IFlowSocket<T> where T : struct, ISubstance, ISubstanceOperations<T>
    {
        public double MaxFlowVolume { get; }

        public IVolumeContainer<T> Container { get; }

        public IFlowSocket<T>? ConnectedSocket { get; }

        public bool IsConnected => ConnectedSocket is not null;

        public T OtherSubstance =>
            ConnectedSocket is not null
            ? ConnectedSocket.Container.StoredSubstance
            : throw new InvalidOperationException();

        public void Connect (IFlowSocket<T> socket);

        public void Disconnect ();

        public void Push (T substance);
    }
}
