#nullable enable

using System;

namespace Aqua.SocketSystem
{
    public interface IUniversalSocket<TIn, TOut> : IInputSocket<TIn>, IOutputSocket<TOut>
    {
        public void SubscribeTo (IOutputSocket<TIn> socket,
                                 Func<TIn, TOut> inputDataСonvertingFunction,
                                 Func<TIn, TIn>? inputDataModificationFunction = null);
    }
}