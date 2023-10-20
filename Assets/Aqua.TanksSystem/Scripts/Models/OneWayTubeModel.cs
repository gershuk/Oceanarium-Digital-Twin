#nullable enable

using System;

using Aqua.SocketSystem;

using UniRx;

namespace Aqua.TanksSystem
{
    public sealed class OneWayTubeModel<TIn, TOut>
    {
        private readonly ReactiveProperty<TIn> _inProp;
        private readonly ReactiveProperty<TOut> _outProp;

        public IUniversalSocket<TIn, TIn> InSocket { get; }
        public IUniversalSocket<TIn, TOut> OutSocket { get; }

        public OneWayTubeModel (Func<TIn, TOut> inputData—onvertingFunction,
                                Func<TIn, TIn>? inputDataModificationFunction = null)
        {
            _inProp = new();
            _outProp = new();

            OutSocket = new UniversalSocket<TIn, TOut>(_outProp);
            InSocket = new UniversalSocket<TIn, TIn>(_inProp);

            OutSocket.SubscribeTo(InSocket, inputData—onvertingFunction, inputDataModificationFunction);
        }
    }
}