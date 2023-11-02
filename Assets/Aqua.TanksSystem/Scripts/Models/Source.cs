using Aqua.SocketSystem;

namespace Aqua.TanksSystem
{
    public class Source<T>
    {
        protected readonly IUniversalSocket<T, T> _socket;
        public IOutputSocket<T> OutputSocket => _socket;

        public T Value
        {
            get => _socket.ReadOnlyProperty.Value;
            set => _socket.TrySetValue(InputDataModificationFunction(value));
        }

        public Source (T data = default, bool isMulticonnection = false) =>
            _socket = isMulticonnection
            ? new MulticonnectionSocket<T, T>()
            : new UniversalSocket<T, T>(data);

        protected virtual T InputDataModificationFunction (T value) => value;

        public void Subscribe (IOutputSocket<T> outputSocket) => _socket.SubscribeTo(outputSocket, InputDataModificationFunction);
    }
}