#nullable enable

using System;

using Aqua.SocketSystem;

using UniRx;

namespace Aqua.TanksSystem
{
    public sealed class OneWayTubeModel<TIn, TOut>
    {
        private readonly IUniversalSocket<TIn?, TIn?> _inSocket;
        private readonly IConverterSocket<TIn?, TOut?> _outSocket;

        public IInputSocket<TIn?> InSocket => _inSocket;
        public IOutputSocket<TOut?> OutSocket => _outSocket;

        public OneWayTubeModel (Func<TIn?, TOut?> inputData—onvertingFunction,
                                Func<TIn?, TIn?>? inputDataModificationFunction = null)
        {
            _outSocket = new ConverterSocket<TIn?, TOut?>();
            _inSocket = new UniversalSocket<TIn?, TIn?>();

            _outSocket.SubscribeTo(_inSocket, inputData—onvertingFunction, inputDataModificationFunction);
        }
    }
}