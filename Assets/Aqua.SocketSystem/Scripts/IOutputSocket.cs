#nullable enable

using UniRx;

namespace Aqua.SocketSystem
{
    public interface IOutputSocket<TOut>
    {
        public IReadOnlyReactiveProperty<TOut?> ReadOnlyProperty { get; }

        public TOut GetValue ();

        public void Register (IInputSocket<TOut?> socket);

        public bool TrySetValue (TOut? value);

        public void Unregister (IInputSocket<TOut?> socket);
    }
}