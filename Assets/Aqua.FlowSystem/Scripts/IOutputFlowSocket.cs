#nullable enable

using System;
using System.Collections.Generic;

namespace Aqua.FlowSystem
{
    public interface IOutputFlowSocket<T> where T : struct, ISubstance, ISubstanceOperations<T>
    {
        public event Action<(T stored, T taked)>? OnDrain;
        public double MaxOutputVolum { get; }

        public double MaxStoredVolum { get; }

        public T StoredSubstance { get; }

        public IReadOnlyCollection<OutputLink<T>> InputLinks { get; }

        public void ConnectToInput (IInputFlowSocket<T> socket, float coefficient);

        public void DisconnectFromInput (IInputFlowSocket<T> socket);

        public void Fill(double requestVolum, out T remainValue, double eps = 1e-2);

        public void Drain (double volum, out T returnedValue);

        public void AddSubstance (T substance, out T remainValue);
    }
}
