using System;

using Aqua.SocketSystem;

namespace Aqua.TanksSystem.Models
{
    public sealed class SimpleValveModel
    {
        private readonly IUniversalSocket<float, float> _socket;
        public IOutputSocket<float> OutputSocket => _socket;

        public float Value
        {
            get => _socket.ReadOnlyProperty.Value;
            set => _socket.TrySetValue(InputDataModificationFunction(value));
        }

        public SimpleValveModel () => _socket = new UniversalSocket<float, float>();

        private float InputDataModificationFunction (float value) =>
                    value is >= 0 and <= 1
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value));

        public void Subscribe (IOutputSocket<float> outputSocket) => _socket.SubscribeTo(outputSocket, InputDataModificationFunction);
    }
}