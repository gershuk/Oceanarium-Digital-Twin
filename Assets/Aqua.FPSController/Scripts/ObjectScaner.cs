#nullable enable

using Aqua.Items;
using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.FPSController
{
    public class ObjectScaner : MonoBehaviour
    {
        private bool _isInited = false;

        [SerializeField]
        private FPSCamera _fpsCamera;

        [SerializeField]
        private float _distanceOfItemInteraction = 5;

        private MulticonnectionSocket<IInfo?, IInfo?> _observedObjectSocket { get; set; }
        public IOutputSocket<IInfo?> ObservedObjectSocket => _observedObjectSocket;

        public void ForceInit ()
        {
            if (_isInited)
                return;

            _observedObjectSocket = new();

            _isInited = true;
        }

        private void Update ()
        {
            _observedObjectSocket.TrySetValue(Physics.Raycast(_fpsCamera.Camera.transform.position,
                                                   _fpsCamera.Camera.transform.TransformDirection(Vector3.forward),
                                                   out var hit,
                                                   _distanceOfItemInteraction)
                ? hit.transform.gameObject.GetComponent<IInfo>()
                : null);
        }

        private void Awake ()
        {
            ForceInit();
        }
    }
}
