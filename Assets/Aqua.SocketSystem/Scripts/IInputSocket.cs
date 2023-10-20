#nullable enable

using System;

namespace Aqua.SocketSystem
{
    public interface IInputSocket<TIn>
    {
        public void SubscribeTo (IOutputSocket<TIn> socket, Func<TIn, TIn>? inputDataModificationFunction = null);

        public void UnsubscribeFrom (IOutputSocket<TIn> socket);
    }
}