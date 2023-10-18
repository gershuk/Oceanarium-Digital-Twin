#nullable enable

namespace Aqua.SocketSystem
{
    public interface IUniversalSocket<T> : IReadOnlySocket<T>, IWriteOnlySocket<T>
    {
        public void SubscribeTwoWays(IUniversalSocket<T> socket);
        public void UnsubscribeTwoWays (IUniversalSocket<T> socket);
    }
}