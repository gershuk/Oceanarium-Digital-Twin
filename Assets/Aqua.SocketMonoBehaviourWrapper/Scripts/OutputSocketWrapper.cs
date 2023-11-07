using System.Collections.Generic;

using UnityEngine;

namespace Aqua.SocketMonoBehaviourWrapper
{
    public sealed class OutputSocketWrapper : BaseSocketWrapper
    {
        [SerializeField]
        private List<InputSocketWrapper> _subscribers = new();

        public IReadOnlyList<InputSocketWrapper> Subscribers => _subscribers;
    }
}