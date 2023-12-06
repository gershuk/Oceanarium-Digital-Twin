using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Aqua.FPSController
{
    public class FPSCamera : MonoBehaviour
    {
        [SerializeField]
        private bool _isCameraChildOfPlayerBody = false;

        [SerializeField]
        private Camera _camera;

        private Transform _cameraTransform;
        private Vector2 _mouseMovement;

        #region Camera transfrom properties

        [Header("Camera transfrom properties")]
        [SerializeField]
        private Transform _cameraPosition;

        [SerializeField]
        private float _maxDownAngle = -80;

        [SerializeField]
        private float _maxUpAngle = 80;

        #endregion Camera transfrom properties

        #region Input properties

        [Header("Input properties")]
        [SerializeField]
        private InputActionReference _mouseMoveAction;

        [SerializeField]
        private Transform _player;

        [SerializeField]
        private float _sensitivity = 0.2f;

        #endregion Input properties

        #region Camera rotation
        private float _rotX = 0.0f;
        private float _rotY = 0.0f;
        private float _rotZ = 0.0f;
        #endregion Camera rotation

        public Camera Camera => _camera;

        private void Awake ()
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();

            _cameraTransform = _camera.transform;

            if (_isCameraChildOfPlayerBody)
            {
                _cameraTransform.position = _cameraPosition.position;
                _cameraTransform.rotation = _cameraPosition.rotation;
            }
        }

        private IEnumerator IShake (float mag, float dur)
        {
            var wfeof = new WaitForEndOfFrame();
            for (var t = 0.0f; t <= dur; t += Time.deltaTime)
            {
                _rotZ = Random.Range(-mag, mag) * ((t / dur) - 1.0f);
                yield return wfeof;
            }
            _rotZ = 0.0f;
        }

        private void Update ()
        {
            _mouseMovement = _mouseMoveAction.action.ReadValue<Vector2>() * _sensitivity;

            _rotX -= _mouseMovement.y;
            _rotX = Mathf.Clamp(_rotX, _maxDownAngle, _maxUpAngle);
            _rotY += _mouseMovement.x;   

            if (!_isCameraChildOfPlayerBody)
            {
                _cameraTransform.rotation = Quaternion.Euler(_rotX, _rotY, _rotZ);
                _cameraTransform.position = _cameraPosition.position;
            }
            else
            {
                _cameraTransform.localEulerAngles = Vector3.right * _rotX;
            }

            _player.Rotate(Vector3.up * _mouseMovement.x);
        }

        public void Shake (float magnitude, float duration) => StartCoroutine(IShake(magnitude, duration));
    }
}