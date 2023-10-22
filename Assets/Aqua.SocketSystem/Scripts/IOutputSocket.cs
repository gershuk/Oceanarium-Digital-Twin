#nullable enable

using UniRx;

namespace Aqua.SocketSystem
{
    public interface IOutputSocket<TOut>
    {
        public IReadOnlyReactiveProperty<TOut?> ReadOnlyProperty { get; }

        public void Register (IInputSocket<TOut?> socket);

        public void Unregister (IInputSocket<TOut?> socket);
    }
}