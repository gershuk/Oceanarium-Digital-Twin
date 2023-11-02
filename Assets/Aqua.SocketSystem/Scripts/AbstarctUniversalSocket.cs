#nullable enable

using System;

using UniRx;

namespace Aqua.SocketSystem
{
    public abstract class AbstarctUniversalSocket<TIn, TOut> : IUniversalSocket<TIn?, TOut?>, IDisposable
    {
        protected readonly CompositeDisposable _mainDisposable = new();

        protected bool _disposedValue;

        protected ReactiveProperty<TOut?> Property { get; set; }

        public Func<TIn?, TIn?>? MainInputDataModificationFunction { get; protected set; }

        public IReadOnlyReactiveProperty<TOut?> ReadOnlyProperty => Property;

        public AbstarctUniversalSocket (ReactiveProperty<TOut?>? property = null) => Property = property ?? new();

        public AbstarctUniversalSocket (TOut? value = default) => Property = new ReactiveProperty<TOut?>(value);

        ~AbstarctUniversalSocket () => Dispose(false);

        protected virtual void Dispose (bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _mainDisposable.Dispose();
                }

                _disposedValue = true;
            }
        }

        protected abstract void RegisterMainPublisher (IOutputSocket<TIn?> socket);

        protected virtual void ResetMainDataFunction () => MainInputDataModificationFunction = null;

        protected abstract void UnregisterMainPublisher (IOutputSocket<TIn?> socket);

        protected virtual void UpdateData (TIn? value)
        {
            var mValue = MainInputDataModificationFunction != null
                             ? MainInputDataModificationFunction(value)
                             : value;

            //ToDo : Refactor this
            if (mValue == null && (Property.Value == null || Property.Value.GetType().IsClass))
            {
                Property.Value = default;
                return;
            }

            Property.Value = value is TOut v
                                ? v
                                : throw new InvalidCastException();
        }

        public void Dispose ()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void Register (IInputSocket<TOut?> socket);

        public void SubscribeTo (IOutputSocket<TIn?> socket, Func<TIn?, TIn?>? inputDataModificationFunction = null)
        {
            RegisterMainPublisher(socket);
            socket.Register(this);
            MainInputDataModificationFunction = inputDataModificationFunction;
            socket.ReadOnlyProperty.Subscribe(UpdateData).AddTo(_mainDisposable);
        }

        public abstract bool TrySetValue (TOut? value);

        public abstract void Unregister (IInputSocket<TOut?> socket);

        public void UnsubscribeFrom (IOutputSocket<TIn?> socket)
        {
            UnregisterMainPublisher(socket);
            socket.Unregister(this);
            ResetMainDataFunction();
            _mainDisposable.Clear();
        }
    }
}