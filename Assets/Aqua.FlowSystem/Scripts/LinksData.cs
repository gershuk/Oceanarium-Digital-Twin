using System;

namespace Aqua.FlowSystem
{
    public readonly struct InputLink<T> where T: struct, ISubstance
    {
        public readonly IOutputFlowSocket<T> Socket;

        public readonly double Coefficient;

        public InputLink (IOutputFlowSocket<T> socket, double coefficient)
        {
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            Coefficient = coefficient;
        }

        public void Deconstruct (out IOutputFlowSocket<T> socket, out double coefficient)
        {
            (socket, coefficient) = (Socket, Coefficient);
        }
    }

    public readonly struct OutputLink<T> where T : struct, ISubstance
    {
        public readonly IInputFlowSocket<T> Socket;

        public readonly double Coefficient;

        public OutputLink (IInputFlowSocket<T> socket, double coefficient)
        {
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            Coefficient = coefficient;
        }

        public void Deconstruct(out IInputFlowSocket<T> socket, out double coefficient)
        {
            (socket,coefficient) = (Socket, Coefficient);
        }
    }
}
