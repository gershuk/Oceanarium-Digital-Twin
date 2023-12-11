#nullable enable

namespace Aqua.FlowSystem
{
    public interface IFlowSocket<T> where T : struct, ISubstance, ISubstanceOperations<T>
    {
        public double MaxFlowVolume { get; }

        public double FlowCoefficient { get; }

        public IVolumeContainer<T> Container { get; }

        public IFlowSocket<T>? ConnectedSocket { get; }

        public IVolumeContainer<T> ConnectedContainer { get; }

        public T ConnectedSubstance { get; }

        public bool IsConnected { get; }

        public void Connect (IFlowSocket<T> socket);

        public void Disconnect ();

        public void SetFlowCoefficient (double coefficient);

        public void PassivePush (T substance);

        public T ActivePush (T substance);
    }
}
