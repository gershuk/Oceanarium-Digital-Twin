#nullable enable

using System;

using UniRx;

namespace Aqua.SocketSystem
{
    public class UniversalSocket<TIn, TOut> : IUniversalSocket<TIn, TOut>, IDisposable
    {
        protected readonly CompositeDisposable _disposable = new();

        protected bool _disposedValue;

        protected IOutputSocket<TIn>? _mainPublisher;
        protected IInputSocket<TOut>? _mainSubscriber;

        protected ReactiveProperty<TOut?> Property { get; set; }

        public Func<TIn, TIn>? InputDataModificationFunction { get; protected set; }
        public Func<TIn, TOut>? InputData—onvertingFunction { get; protected set; }

        public IReadOnlyReactiveProperty<TOut?> ReadOnlyProperty => Property;

        public UniversalSocket (ReactiveProperty<TOut?> property) => Property = property ?? throw new ArgumentNullException(nameof(property));

        protected void Dispose (bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _disposable.Dispose();
                }

                _disposedValue = true;
            }
        }

        protected void Register (IOutputSocket<TIn> socket)
        {
            if (_mainPublisher != null)
                throw new Exception($"{nameof(_mainPublisher)} != null");
            _mainPublisher = socket;
        }

        protected void ResetDataFunction ()
        {
            InputDataModificationFunction = null;
            InputData—onvertingFunction = null;
        }

        protected void Unregister (IOutputSocket<TIn> socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (socket != _mainPublisher)
                throw new Exception($"{nameof(socket)} != {nameof(_mainPublisher)}");

            _mainPublisher = null;
        }

        protected void UpdateData (TIn value)
        {
            var mValue = InputDataModificationFunction != null
                             ? InputDataModificationFunction(value)
                             : value;
            //ToDo : Refactor this
            if (mValue == null && (Property.Value == null || Property.Value.GetType().IsClass))
            {
                Property.Value = default;
                return;
            }

            Property.Value = InputData—onvertingFunction != null
                           ? InputData—onvertingFunction(value)
                           : value is TOut v
                                ? v
                                : throw new InvalidCastException();
        }

        public void Dispose ()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Register (IInputSocket<TOut> socket)
        {
            if (_mainSubscriber != null)
                throw new Exception($"{nameof(_mainSubscriber)} != null");
            _mainSubscriber = socket;
        }

        public void SubscribeTo (IOutputSocket<TIn> socket,
                                         Func<TIn, TOut> inputData—onvertingFunction,
                                 Func<TIn, TIn>? inputDataModificationFunction = null)
        {
            Register(socket);
            socket.Register(this);
            InputDataModificationFunction = inputDataModificationFunction;
            InputData—onvertingFunction = inputData—onvertingFunction;
            socket.ReadOnlyProperty.Subscribe(UpdateData).AddTo(_disposable);
        }

        public void SubscribeTo (IOutputSocket<TIn> socket, Func<TIn, TIn>? inputDataModificationFunction = null)
        {
            Register(socket);
            socket.Register(this);
            InputDataModificationFunction = inputDataModificationFunction;
            socket.ReadOnlyProperty.Subscribe(UpdateData).AddTo(_disposable);
        }

        public void Unregister (IInputSocket<TOut> socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (socket != _mainSubscriber)
                throw new Exception($"{nameof(socket)} != {nameof(_mainSubscriber)}");

            _mainSubscriber = null;
        }

        public void UnsubscribeFrom (IOutputSocket<TIn> socket)
        {
            Unregister(socket);
            socket.Unregister(this);
            ResetDataFunction();
            _disposable.Clear();
        }
    }
}