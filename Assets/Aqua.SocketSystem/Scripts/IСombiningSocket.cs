#nullable enable

using System;

namespace Aqua.SocketSystem
{
    public interface IÑombiningSocket<TIn1, TIn2, TOut> : IOutputSocket<TOut>
    {
        public void SubscribeTo ((IOutputSocket<TIn1>? first, IOutputSocket<TIn2>? second) publishers,
                                 Func<TIn1, (IOutputSocket<TIn1>? first, IOutputSocket<TIn2>? second)>? combineFunction);

        public void UnsubscribeFrom ((IOutputSocket<TIn1>? first, IOutputSocket<TIn2>? second) publishers);
    }
}