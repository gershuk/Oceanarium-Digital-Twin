using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

using UnityEngine;

using static PlasticPipe.PlasticProtocol.Client.ConnectionCreator.PlasticProtoSocketConnection;

namespace Aqua.FlowSystem
{
    public class WaterPipe : MonoBehaviour, IVolumeContainer<Water>
    {
        [SerializeField]
        private double _maxVolume;
        [SerializeField]
        private Water _substance;

        private List<WaterSocket> _sockets;

        private void Awake ()
        {
            _sockets = GetComponentsInChildren<WaterSocket>().ToList();
        }

        private void FixedUpdate ()
        {
            PassiveFlow();
        }

        public double MaxVolume => _maxVolume;

        public Water StoredSubstance => _substance;

        public bool IsFull => _substance.Volume >= _maxVolume; // TODO Use comparator

        public void AddSubstance (Water substance)
        {
            _substance = _substance.Combine(substance);
        }

        public IReadOnlyCollection<IFlowSocket<Water>> Sockets => _sockets;

        public void PassiveFlow ()
        {
            if (_substance.IsVolumeApproximatelyEqual(0.0, 1e-6)) // TODO Define and use eps
                return;

            var flowSockets = _sockets.FindAll((socket) => IsSocketAvailableToFlow(socket));
            if (flowSockets.Count == 0)
                return;

            var count = flowSockets.Count + 1;
            var amount = _substance.Volume;
            foreach (IFlowSocket<Water> socket in flowSockets)
                amount += socket.OtherSubstance.Volume;

            var average = amount / count;
            if (average > _substance.Volume)
                throw new InvalidOperationException();

            var delta = (_substance.Volume - average) * Time.fixedDeltaTime;

            var coefficients = new double[count];
            coefficients[0] = (_substance.Volume - delta) / _substance.Volume;

            for (var i = 1; i < count; ++i)
                coefficients[i] = (1 - coefficients[0]) / (coefficients.Length - 1);

            var substances = _substance.Separate(coefficients);
            _substance = substances[0];
            for (var i = 0; i < flowSockets.Count; ++i)
                flowSockets[i].Push(substances[i + 1]);
        }

        private bool IsSocketAvailableToFlow (IFlowSocket<Water> socket) =>
            socket.IsConnected &&
            !socket.ConnectedSocket.Container.IsFull &&
            socket.OtherSubstance.Volume < _substance.Volume; // TODO Define and use eps

    }
}
