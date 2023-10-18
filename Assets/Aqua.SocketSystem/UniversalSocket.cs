#nullable enable

using System;

using UniRx;

namespace Aqua.SocketSystem
{
    public sealed class UniversalSocket<T> : IUniversalSocket<T>, IDisposable
    {
        private readonly CompositeDisposable _disposable = new();

        private bool _disposedValue;

        private IWriteOnlySocket<T>? _writer;

        private ReactiveProperty<T> Property { get; set; }

        public Func<T, T>? ConverterFunction { get; }

        public IReadOnlyReactiveProperty<T> ReadOnlyProperty => Property;

        public UniversalSocket (ReactiveProperty<T> property, Func<T, T>? converterFunction = null)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            ConverterFunction = converterFunction;
        }

        private void Dispose (bool disposing)
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

        private void Register (IWriteOnlySocket<T> socket)
        {
            if (_writer != null)
                throw new Exception("_writer != null");
            _writer = socket;
        }

        private void Unegister (IWriteOnlySocket<T> socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (socket != _writer)
                throw new Exception("socket != _writer");

            _writer = null;
        }

        public void Dispose ()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SubscribeTo (IWriteOnlySocket<T> socket)
        {
            Register(socket);
            _ = socket.ReadOnlyProperty.Subscribe(value => Property.Value = ConverterFunction is not null
                                                                        ? ConverterFunction(value)
                                                                        : value
                                             ).AddTo(_disposable);
        }

        public void UnsubscribeFrom (IWriteOnlySocket<T> socket)
        {
            Unegister(socket);
            _disposable.Clear();
        }

        public void SubscribeTwoWays (IUniversalSocket<T> socket)
        {
            SubscribeTo (socket);
            socket.SubscribeTo (this);
        }

        public void UnsubscribeTwoWays (IUniversalSocket<T> socket)
        {
            UnsubscribeFrom(socket);
            socket.UnsubscribeFrom(this);
        }
    }
}