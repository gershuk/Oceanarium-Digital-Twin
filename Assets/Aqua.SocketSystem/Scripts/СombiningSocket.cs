#nullable enable

using System;

using UniRx;

namespace Aqua.SocketSystem
{
    public sealed class СombiningSocket<TIn1, TIn2, TOut> : UniversalSocket<TIn1, TOut>, IInputSocket<TIn2>
    {
        public СombiningSocket (ReactiveProperty<TOut?> property) : base(property)
        {
        }

        public void SubscribeTo (IOutputSocket<TIn2> socket, Func<TIn2, TIn2>? inputDataModificationFunction = null) => throw new NotImplementedException();

        public void UnsubscribeFrom (IOutputSocket<TIn2> socket) => throw new NotImplementedException();
    }
}