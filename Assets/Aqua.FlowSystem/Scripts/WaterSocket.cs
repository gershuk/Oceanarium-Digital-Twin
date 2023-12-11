#nullable enable

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Aqua.FlowSystem
{
    public class WaterSocket : MonoBehaviour, IFlowSocket<Water>
    {
        [SerializeField]
        private double _maxFlowVolume;
        [SerializeField]
        private double _flowCoefficient;
        [SerializeField]
        private WaterSocket? _connection;
        private IVolumeContainer<Water> _parentContainer;
        private IFlowSocket<Water>? _connectedSocket;

        private void Awake ()
        {
            _parentContainer = GetComponentInParent<IVolumeContainer<Water>>();
            if (_parentContainer == null)
                throw new NullReferenceException();
            if (_connection is not null)
            {
                Connect(_connection);
            }
        }

        public double MaxFlowVolume => _maxFlowVolume;

        public double FlowCoefficient => _flowCoefficient;

        public IVolumeContainer<Water> Container => _parentContainer;

        public IFlowSocket<Water>? ConnectedSocket => _connectedSocket;

        public IVolumeContainer<Water> ConnectedContainer =>
            _connectedSocket is not null
            ? _connectedSocket.Container
            : throw new InvalidOperationException();

        public Water ConnectedSubstance => ConnectedContainer.StoredSubstance;

        public bool IsConnected => _connectedSocket != null;

        public void Connect (IFlowSocket<Water> socket)
        {
            if (_connectedSocket is null)
            {
                _connectedSocket = socket;
                _connection = socket as WaterSocket; // Only for debug in the editor
                socket.Connect(this);
            }
            else if (socket.ConnectedSocket != this as IFlowSocket<Water>)
            {
                throw new InvalidOperationException();
            }
        }

        public void Disconnect () =>
            _connectedSocket = _connectedSocket is not null ? null : throw new InvalidOperationException();

        public void SetFlowCoefficient (double coefficient) => _flowCoefficient = coefficient;

        public void PassivePush (Water substance)
        {
            if (_connectedSocket is null)
                throw new InvalidOperationException();

            _connectedSocket.Container.AddSubstance(substance);
        }

        public Water ActivePush (Water substance)
        {
            if (_connectedSocket is null)
                throw new InvalidOperationException();

            //var maxFlowRemainSubstance =

            var container = _connectedSocket.Container;
            substance = container.AddSubstance(substance);

            if (substance.IsVolumeApproximatelyEqual(0))
                return default;

            var sockets = new List<IFlowSocket<Water>>(container.Sockets.Count);
            var coefficients = new List<double>(container.Sockets.Count);
            foreach (var socket in container.Sockets)
            {
                if (socket.IsConnected &&
                    socket != ConnectedSocket &&
                    socket.MaxFlowVolume != 0.0)
                {
                    sockets.Add(socket);
                    coefficients.Add(socket.MaxFlowVolume);
                }
            }

            Utils.NormalizeCoefficients(coefficients);
            var substaceParts = substance.Separate(coefficients.ToArray());

            Water remains = default;
            for (var i = 0; i < sockets.Count; ++i)
                remains = sockets[i].ActivePush(substaceParts[i].Combine(remains));

            return remains;
        }
    }
}
