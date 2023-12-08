﻿#nullable enable

using System;

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

        public void Push (Water substance)
        {
            if (_connectedSocket is null)
                throw new InvalidOperationException ();

            _connectedSocket.Container.AddSubstance(substance);
        }
    }
}
