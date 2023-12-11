using System.Collections.Generic;

namespace Aqua.FlowSystem
{
    public interface IVolumeContainer<T> where T : struct, ISubstance, ISubstanceOperations<T>
    {
        public double MaxVolume { get; }

        public double FreeVolume { get; }

        public T StoredSubstance { get; }

        public bool IsFull { get; }

        public T AddSubstance (T substance);

        public IReadOnlyCollection<IFlowSocket<T>> Sockets { get; }
    }
}
