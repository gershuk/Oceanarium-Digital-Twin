#nullable enable

using System;

using Aqua.SocketSystem;

using UniRx;

namespace Aqua.TanksSystem
{
    public sealed class OneWayTubeModel<TIn, TOut>
    {
        private readonly ReactiveProperty<TIn?> _inProp;
        private readonly ReactiveProperty<TOut?> _outProp;
        private readonly IUniversalSocket<TIn?, TIn?> _inSocket;
        private readonly IConverterSocket<TIn?, TOut?> _outSocket;

        public IInputSocket<TIn?> InSocket => _inSocket;
        public IOutputSocket<TOut?> OutSocket => _outSocket;

        public OneWayTubeModel (Func<TIn?, TOut?> inputData—onvertingFunction,
                                Func<TIn?, TIn?>? inputDataModificationFunction = null)
        {
            _inProp = new();
            _outProp = new();

            _outSocket = new ConverterSocket<TIn?, TOut?>(_outProp);
            _inSocket = new UniversalSocket<TIn?, TIn?>(_inProp);

            _outSocket.SubscribeTo(_inSocket, inputData—onvertingFunction, inputDataModificationFunction);
        }
    }
}