using UnityEngine;
using UnityEngine.InputSystem;

namespace Aqua.FPSController
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        private Vector3 _inputForce;
        private float _lastJumpTime = 0f;
        private float _prevY;
        private Rigidbody _rigidbody;

        #region Movement properties

        [Header("Movement properties")]
        [SerializeField]
        private float _changeInStageSpeed = 10.0f;

        [SerializeField]
        private Transform _groundChecker;

        [SerializeField]
        private float _groundCheckerDist = 0.2f;

        [SerializeField]
        private float _maximumPlayerSpeed = 150.0f;

        [SerializeField]
        private float _runSpeed = 12.0f;

        [SerializeField]
        private float _walkSpeed = 8.0f;

        #endregion Movement properties

        #region Jump properties

        [Header("Jump properties")]
        [SerializeField]
        private float _jumpCooldown = 1.0f;

        [SerializeField]
        private float _jumpForce = 500.0f;

        #endregion Jump properties

        #region Input actions

        [Header("Input actions")]
        [SerializeField]
        private InputActionReference _jumpAction;

        [SerializeField]
        private InputActionReference _moveAction;

        [SerializeField]
        private InputActionReference _sprintAction;

        #endregion Input actions

        private bool CanJump => IsGrounded && (Time.time - _lastJumpTime) > _jumpCooldown;

        public bool EnableMovement { get; set; } = true;

        public bool IsGrounded { get; private set; } = false;

        private static Vector3 ClampMag (Vector3 vec, float maxMag)
        {
            if (vec.sqrMagnitude > maxMag * maxMag)
                vec = vec.normalized * maxMag;
            return vec;
        }

        private static Vector3 ClampSqrMag (Vector3 vec, float sqrMag)
        {
            if (vec.sqrMagnitude > sqrMag)
                vec = vec.normalized * Mathf.Sqrt(sqrMag);
            return vec;
        }

        private void FixedUpdate ()
        {
            IsGrounded = (Mathf.Abs(_rigidbody.velocity.y - _prevY) < .1f) &&
                (Physics.OverlapSphere(_groundChecker.position, _groundCheckerDist).Length > 1); // > 1 because it also counts the player

            _prevY = _rigidbody.velocity.y;

            _rigidbody.velocity = ClampMag(_rigidbody.velocity, _maximumPlayerSpeed);

            if (!EnableMovement)
                return;

            AddMoveForce(_moveAction.action.ReadValue<Vector2>(), _sprintAction.action.IsInProgress());

            if (IsGrounded)
            {
                if (_jumpAction.action.IsPressed() && CanJump)
                {
                    AddJumpForce();
                    _lastJumpTime = Time.time;
                }

                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, _inputForce, _changeInStageSpeed * Time.fixedDeltaTime);
            }
            else
            {
                _rigidbody.velocity = ClampSqrMag(_rigidbody.velocity + (_inputForce * Time.fixedDeltaTime), _rigidbody.velocity.sqrMagnitude);
            }
        }

        private void Start () => _rigidbody = GetComponent<Rigidbody>();

        public void AddJumpForce () => _rigidbody.AddForce(_jumpForce * _rigidbody.mass * Vector3.up);

        public void AddMoveForce (Vector2 movement, bool sprint) =>
                    _inputForce = ((transform.forward * movement.y)
                                    + (transform.right * movement.x)).normalized * (sprint ? _runSpeed : _walkSpeed);
    }
}