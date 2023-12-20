#nullable enable

using System;

using Aqua.SocketSystem;

namespace Aqua.TanksSystem.Models
{
    public class OneWayTubeWhithValveModel<TIn, TOut>
    {
        private readonly CombiningSocket<TIn?, float, TIn?> _inSocket;
        private readonly IConverterSocket<TIn?, TOut?> _outSocket;

        public IInputSocket<TIn?> InSocket => _inSocket;
        public IOutputSocket<TOut?> OutSocket => _outSocket;
        public IInputSocket<float> ValveSocket => _inSocket;

        public OneWayTubeWhithValveModel (SimpleValveModel? simpleValveModel = null,
                                          Func<TIn?, TOut?>? inputData—onvertingFunction = null,
                                          Func<TIn?, TIn?>? inputDataModificationFunction = null,
                                          Func<TIn?, float, TIn?>? combineFunction = null)
        {
            _outSocket = new ConverterSocket<TIn?, TOut?>();
            _inSocket = new CombiningSocket<TIn?, float, TIn?>(combineFunction: combineFunction);

            _outSocket.SubscribeTo(_inSocket, inputData—onvertingFunction, inputDataModificationFunction);

            if (simpleValveModel != null)
                _inSocket.SubscribeTo(simpleValveModel.OutputSocket);
        }
    }
}