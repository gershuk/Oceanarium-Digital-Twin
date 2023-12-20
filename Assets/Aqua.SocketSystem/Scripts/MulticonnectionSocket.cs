#nullable enable

using System;
using System.Collections.Generic;

using UniRx;

namespace Aqua.SocketSystem
{
    public class MulticonnectionSocket<TIn, TOut> : AbstarctUniversalSocket<TIn?, TOut?>
    {
        protected HashSet<IOutputSocket<TIn?>> _publishers;
        protected HashSet<IInputSocket<TOut?>> _subscribers;

        public MulticonnectionSocket () : base(property: default)
        {
            _publishers = new();
            _subscribers = new();
        }

        public MulticonnectionSocket (TOut? value = default) : base(value)
        {
            _publishers = new();
            _subscribers = new();
        }

        public MulticonnectionSocket (ReactiveProperty<TOut?>? property = null) : base(property)
        {
            _publishers = new();
            _subscribers = new();
        }

        protected override void RegisterMainPublisher (IOutputSocket<TIn?> socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (_publishers.Contains(socket))
                throw new Exception($"{socket} already added");
            _publishers.Add(socket);
        }

        protected override void UnregisterMainPublisher (IOutputSocket<TIn?> socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            _publishers.Remove(socket);
        }

        public override void Register (IInputSocket<TOut?> socket)
        {
            if (_subscribers.Contains(socket))
                throw new Exception($"{socket} already added");

            _subscribers.Add(socket);
        }

        public override bool TrySetValue (TOut? value)
        {
            if (_publishers.Count == 0)
                Property.Value = value;
            return _publishers.Count == 0;
        }

        public override void Unregister (IInputSocket<TOut?> socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            _subscribers.Remove(socket);
        }
    }
}