#nullable enable

using System;

namespace Aqua.SocketSystem
{
    public interface IConverterSocket<TIn, TOut> : IUniversalSocket<TIn, TOut>
    {
        public void SubscribeTo (IOutputSocket<TIn?> socket,
                                 Func<TIn?, TOut?> inputDataСonvertingFunction,
                                 Func<TIn?, TIn?>? inputDataModificationFunction = null);
    }
}