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

        public bool IsFull => !_substance.IsVolumeApproximatelyLess(_maxVolume, 1e-3);  // TODO Define and use eps

        public void AddSubstance (Water substance) => _substance = _substance.Combine(substance);

        public IReadOnlyCollection<IFlowSocket<Water>> Sockets => _sockets;

        public void PassiveFlow ()
        {
            if (_substance.IsVolumeApproximatelyEqual(0.0, 1e-3)) // TODO Define and use eps
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
            for(var i = 0; i < count; ++i)
            {
                deltas[i] = Math.Min(average - passiveFlowSockets[i].ConnectedSubstance.Volume,
                                     passiveFlowSockets[i].ConnectedContainer.FreeVolume)
                                        * Time.fixedDeltaTime;
            }

            deltas[count] = _substance.Volume - deltas.Sum();
            var coefficients = GetNormalizedCoefficients(deltas);

            Debug.Log(count + " " + coefficients.Sum());

            var substances = _substance.Separate(coefficients);
            for (var i = 0; i < count; ++i)
                passiveFlowSockets[i].Push(substances[i]);

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

        private double[] GetNormalizedCoefficients (double[] values)
        {
            var cefficiensSum = 0.0;

            foreach (var coef in values)
                cefficiensSum += coef;

            var coefficients = new double[values.Length];

            for (var i = 0; i < values.Length; ++i)
                coefficients[i] = values[i] / cefficiensSum;

            return coefficients;
        }

        private bool IsPassiveFlowSocket (IFlowSocket<Water> socket) =>
            !socket.ConnectedSocket.Container.IsFull &&
            socket.ConnectedSubstance.IsVolumeApproximatelyLess(_substance.Volume, 1e-3);  // TODO Define and use eps
    }
}
