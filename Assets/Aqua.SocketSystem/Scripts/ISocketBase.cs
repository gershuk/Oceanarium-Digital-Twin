#nullable enable

using UniRx;

namespace Aqua.SocketSystem
{
    public interface ISocketBase<T>
    {
        public IReadOnlyReactiveProperty<T> ReadOnlyProperty { get; }
    }
}