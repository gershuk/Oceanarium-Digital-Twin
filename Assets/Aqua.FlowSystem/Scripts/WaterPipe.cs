using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

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

        private void Start ()
        {
            _sockets.RemoveAll((socket) => !socket.IsConnected);
            CalcCoefficients();
        }

        private void FixedUpdate ()
        {
            PassiveFlow();
        }

        public double MaxVolume => _maxVolume;

        public double FreeVolume => _maxVolume - _substance.Volume;

        public Water StoredSubstance => _substance;

        public bool IsFull => !_substance.IsVolumeApproximatelyLess(_maxVolume);

        public Water AddSubstance (Water substance)
        {
            _substance = _substance.Combine(substance);
            if (_substance.Volume > MaxVolume)
            {
                var coefs = new double[2];
                coefs[0] = MaxVolume / _substance.Volume;
                coefs[1] = 1 - coefs[0];
                var substanceParts = _substance.Separate(coefs);
                _substance = substanceParts[0];
                return substanceParts[1];
            }

            return default;
        }

        public IReadOnlyCollection<IFlowSocket<Water>> Sockets => _sockets;

        private void PassiveFlow ()
        {
            if (_substance.IsVolumeApproximatelyEqual(0))
                return;

            var passiveFlowSockets = _sockets.FindAll((socket) => IsPassiveFlowSocket(socket));
            if (passiveFlowSockets.Count == 0)
                return;

            var count = passiveFlowSockets.Count;
            var amount = _substance.Volume;
            foreach (var socket in passiveFlowSockets)
                amount += socket.ConnectedSubstance.Volume;

            var average = amount / (count + 1);

            var deltas = new double[count + 1];
            for (var i = 0; i < count; ++i)
            {
                deltas[i] = Math.Min(average - passiveFlowSockets[i].ConnectedSubstance.Volume,
                                     passiveFlowSockets[i].ConnectedContainer.FreeVolume)
                                        * Time.fixedDeltaTime;
            }

            deltas[count] = _substance.Volume - deltas.Sum();
            Utils.NormalizeCoefficients(deltas);

            var substances = _substance.Separate(deltas);
            for (var i = 0; i < count; ++i)
                passiveFlowSockets[i].PassivePush(substances[i]);

            _substance = substances[count];
        }

        private void CalcCoefficients ()
        {
            var totalFlowVolume = 0.0;

            foreach (var socket in _sockets)
                totalFlowVolume += socket.MaxFlowVolume;

            foreach (var socket in _sockets)
                socket.SetFlowCoefficient(socket.MaxFlowVolume / totalFlowVolume);
        }

        private bool IsPassiveFlowSocket (IFlowSocket<Water> socket) =>
            !socket.ConnectedSocket.Container.IsFull &&
            socket.ConnectedSubstance.IsVolumeApproximatelyLess(_substance.Volume);
    }
}
