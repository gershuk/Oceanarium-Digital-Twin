#nullable enable

using System;
using System.Collections.Generic;

using Aqua.Utils;

namespace Aqua.FlowSystem
{
    public class MultyOutputFlowSocket<T>  where T : struct, ISubstance, ISubstanceOperations<T>
    {
        private readonly SortedSet<OutputLink<T>> _outputLinks;

        public event Action<(T stored, T taked)>? OnDrain;

        public double MaxOutputVolum { get; protected set; }

        public T StoredSubstance {get; protected set;}

        public IReadOnlyCollection<OutputLink<T>> InputLinks => _outputLinks;

        public double MaxStoredVolum {get; protected set;}

        public void ConnectToInput (IInputFlowSocket<T> socket, float coefficient) =>
            _outputLinks.Add(new OutputLink<T>(socket, coefficient));

        public void DisconnectFromInput (IInputFlowSocket<T> socket) =>
            _outputLinks.RemoveWhere(link => link.Socket == socket);

        public void Drain (double volum, out T returnedValue)
        {
            var takedVolum = Math.Min(MaxOutputVolum, Math.Min(StoredSubstance.Volum, volum));
            var seps = StoredSubstance.Separate(takedVolum / volum, 1 - takedVolum / volum);
            returnedValue = seps[0];
            StoredSubstance = seps[1];
            OnDrain?.Invoke((StoredSubstance, returnedValue));
        }

        public void Fill (double requestVolum, out T remainValue, double eps = 1e-2)
        {
            var limitedRequestVolum = Math.Min(Math.Min(StoredSubstance.Volum,requestVolum), MaxOutputVolum);
            remainValue = default;

            var isNotEnough = true;
            var notFull = true;

            var bufferVolum = limitedRequestVolum;
            while (isNotEnough && notFull)
            {
                notFull = false;
                T sended = default;

                //Сделать перещёт остатка, сделать отдачу в зависимости от остатка
                //Переделать так чтобы хранилось значение сколько мы передали на каждый сокет
                //и оно не привышало его максимальную пропускную сопсобность
                foreach (var (socket, coef) in _outputLinks)
                {
                    var seps = StoredSubstance.Separate(bufferVolum * (1 - coef),bufferVolum * coef);
                    socket.Fill(seps[1], out var value);

                    if (!value.IsVolumApproximatelyEqual(seps[1].Volum, eps))
                        notFull = true;

                    remainValue = remainValue.Combine(value);
                    sended.Combine();

                    if (remainValue.IsVolumApproximatelyEqual(requestVolum, eps))
                        isNotEnough = false;
                    break;
                }
            }
        }

        public void AddSubstance (T substance, out T remainValue)
        {
            var delta = MaxStoredVolum - StoredSubstance.Volum;
            var seps = substance.Separate(Math.Min(delta / substance.Volum, 1), Math.Max(1 - delta / substance.Volum, 0));
            StoredSubstance = StoredSubstance.Combine(seps[0]);
            remainValue = seps[1];
        }

        public MultyOutputFlowSocket (double maxStoredVolum = double.MaxValue, double maxOutputVolum = double.MaxValue)
        {
            _outputLinks = new(OutputLinkComparer<T>.Default);
            MaxStoredVolum = maxStoredVolum;
            MaxOutputVolum = maxOutputVolum;
        }
    }

    //Переписать компаратор так чтобы был порядок убывания
    public class OutputLinkComparer<T> : IComparer<OutputLink<T>> where T : struct, ISubstance
    {
        public static OutputLinkComparer<T> Default = new();
        public int Compare (OutputLink<T> x, OutputLink<T> y) => x.Coefficient.CompareTo(y.Coefficient);
    }
}
