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

        public Source (T data = default) => _socket = new UniversalSocket<T, T>(data);

        protected virtual T InputDataModificationFunction (T value) => value;

        public void Subscribe (IOutputSocket<T> outputSocket) => _socket.SubscribeTo(outputSocket, InputDataModificationFunction);
    }
}