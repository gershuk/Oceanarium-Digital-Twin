#nullable enable

using System;

using UniRx;

namespace Aqua.SocketSystem
{
    public class UniversalSocket<TIn, TOut> : AbstarctUniversalSocket<TIn?, TOut?>
    {
        protected IOutputSocket<TIn?>? _mainPublisher;
        protected IInputSocket<TOut?>? _mainSubscriber;

        public UniversalSocket () : base(property: default)
        {
        }

        public UniversalSocket (ReactiveProperty<TOut?>? property = null) : base(property)
        {
        }

        public UniversalSocket (TOut? value = default) : base(value)
        {
        }

        protected override void RegisterMainPublisher (IOutputSocket<TIn?> socket)
        {
            if (_mainPublisher != null)
                throw new Exception($"{nameof(_mainPublisher)} != null");
            _mainPublisher = socket;
        }

        protected override void UnregisterMainPublisher (IOutputSocket<TIn?> socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (socket != _mainPublisher)
                throw new Exception($"{nameof(socket)} != {nameof(_mainPublisher)}");

            _mainPublisher = null;
        }

        public override void Register (IInputSocket<TOut?> socket)
        {
            if (_mainSubscriber != null)
                throw new Exception($"{nameof(_mainSubscriber)} != null");
            _mainSubscriber = socket;
        }

        public override bool TrySetValue (TOut? value)
        {
            if (_mainPublisher == null)
                Property.Value = value;
            return _mainPublisher == null;
        }

        public override void Unregister (IInputSocket<TOut?> socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (socket != _mainSubscriber)
                throw new Exception($"{nameof(socket)} != {nameof(_mainSubscriber)}");

            _mainSubscriber = null;
        }
    }
}