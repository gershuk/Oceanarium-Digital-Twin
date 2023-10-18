#nullable enable

namespace Aqua.SocketSystem
{
    public interface IReadOnlySocket<T> : ISocketBase<T>
    {
        public void SubscribeTo (IWriteOnlySocket<T> socket);

        public void UnsubscribeFrom (IWriteOnlySocket<T> socket);
    }
}