#nullable enable

using System;
using System.Collections.Generic;

namespace Aqua.FlowSystem
{
    public interface IInputFlowSocket<T> where T : struct, ISubstance, ISubstanceOperations<T>
    {
        public double MaxInputVolum{ get; }

        public event Action<T>? OnFill;

        public IReadOnlyCollection<InputLink<T>> InputLinks { get; }
        public void ConnectToOutput (IOutputFlowSocket<T> flowSocket, float coefficient);

        public void DisconnectFromOutput (IOutputFlowSocket<T> socket);

        public void Drain (double requestVolum, out T returnedValue, double eps = 1e-2);

        public void Fill (in T value, out T remainsValue);
    }
}
