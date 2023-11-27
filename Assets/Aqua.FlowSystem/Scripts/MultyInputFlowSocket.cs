using System;
using System.Collections;
using System.Collections.Generic;
#nullable enable

using Aqua.Utils;

using UnityEditor;

namespace Aqua.FlowSystem
{
    public class MultyInputFlowSocket<T> : IInputFlowSocket<T> where T : struct, ISubstance, ISubstanceOperations<T>
    {
        private readonly SortedSet<InputLink<T>> _inputLinks;

        public event Action<T>? OnFill;

        public IReadOnlyCollection<InputLink<T>> InputLinks => _inputLinks;

        public double MaxInputVolum { get; protected set;  }

        public void ConnectToOutput (IOutputFlowSocket<T> flowSocket, float coefficient) => 
            _inputLinks.Add(new InputLink<T>(flowSocket, coefficient));

        public void DisconnectFromOutput (IOutputFlowSocket<T> socket) => _inputLinks.RemoveWhere(link => link.Socket == socket);

        //ѕеределать так чтобы хранилось значение сколько мы получили из каждого сокета
        //и оно не привышало его максимальную пропускную сопсобность
        public void Drain (double requestVolum, out T returnedValue, double eps = 1e-2)
        {
            var limitedRequestVolum = Math.Min(requestVolum, MaxInputVolum);
            returnedValue = default;

            var isNotEnough = true;
            var notEmpty = true;

            var bufferVolum = limitedRequestVolum;
            while (isNotEnough && notEmpty)
            {
                notEmpty = false;

                bufferVolum -= returnedValue.Volum;

                if (bufferVolum <= eps)
                    break;

                foreach (var (socket, coef) in _inputLinks)
                {
                    socket.Drain(bufferVolum * coef, out var value);
                    if (value.Volum > 0)
                        notEmpty = true;

                    returnedValue = returnedValue.Combine(value);

                    if (returnedValue.IsVolumApproximatelyEqual(limitedRequestVolum, eps))
                        isNotEnough = false;
                        break;
                }
            }
        }

        public void Fill (in T value, out T remainsValue)
        {
            // ToDo : Calc remainsValue with OnFill remains + MaxInputVolum
            var div = value.Volum / MaxInputVolum;
            var seps = value.Separate(Math.Min(div, 1), Math.Max(0, div -1));
            OnFill?.Invoke(seps[0]);
            remainsValue = seps[1];
        }

        public MultyInputFlowSocket (double maxInputVolum = double.MaxValue)
        {
            _inputLinks = new(InputLinkComparer<T>.Default);
            MaxInputVolum = maxInputVolum;
        }
    }

    //ѕереписать компаратор так чтобы был пор€док убывани€
    public class InputLinkComparer<T> : IComparer<InputLink<T>> where T : struct, ISubstance
    {
        public static InputLinkComparer<T> Default = new();

        public int Compare (InputLink<T> x, InputLink<T> y) => x.Coefficient.CompareTo(y.Coefficient);
    }
}
