using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SFPSC_PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;

    private bool _enableMovement = true;

    [Header("Movement properties")]
    public float walkSpeed = 8.0f;
    public float runSpeed = 12.0f;
    public float changeInStageSpeed = 10.0f; // Lerp from walk to run and backwards speed
    public float maximumPlayerSpeed = 150.0f;
    public Transform groundChecker;
    public float groundCheckerDist = 0.2f;

    [Header("Jump")]
    public float jumpForce = 500.0f;
    public float jumpCooldown = 1.0f;

    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference sprintAction;
    public InputActionReference jumpAction;

    private void Start ()
    {
        _rb = this.GetComponent<Rigidbody>();
    }

    private bool _isGrounded = false;
    public bool IsGrounded { get { return _isGrounded; } }

    private Vector3 _inputForce;
    private float _prevY;

    private void FixedUpdate ()
    {
        _isGrounded = (Mathf.Abs(_rb.velocity.y - _prevY) < .1f) &&
            (Physics.OverlapSphere(groundChecker.position, groundCheckerDist).Length > 1); // > 1 because it also counts the player
        _prevY = _rb.velocity.y;

        // Clamping speed
        _rb.velocity = ClampMag(_rb.velocity, maximumPlayerSpeed);

        if (!_enableMovement)
            return;
        AddMoveForce(moveAction.action.ReadValue<Vector2>(), sprintAction.action.IsInProgress());
        
        if (_isGrounded)
        {
            if (jumpAction.action.IsPressed())
            {
                AddJumpForce();
            }
            // Ground controller
            _rb.velocity = Vector3.Lerp(_rb.velocity, _inputForce, changeInStageSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Air control
            _rb.velocity = ClampSqrMag(_rb.velocity + _inputForce * Time.fixedDeltaTime, _rb.velocity.sqrMagnitude);
        }
    }

    private static Vector3 ClampSqrMag (Vector3 vec, float sqrMag)
    {
        if (vec.sqrMagnitude > sqrMag)
            vec = vec.normalized * Mathf.Sqrt(sqrMag);
        return vec;
    }

    private static Vector3 ClampMag (Vector3 vec, float maxMag)
    {
        if (vec.sqrMagnitude > maxMag * maxMag)
            vec = vec.normalized * maxMag;
        return vec;
    }

    #region Previous Ground Check
    /*private void OnCollisionStay(Collision collision)
    {
        isGrounded = false;
        Debug.Log(collision.contactCount);
        for(int i = 0; i < collision.contactCount; ++i)
        {
            if (Vector3.Dot(Vector3.up, collision.contacts[i].normal) > .2f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }*/
    #endregion

    public void AddMoveForce (Vector2 movement, bool sprint)
    {
        _inputForce = (transform.forward * movement.y + transform.right * movement.x).normalized * (sprint ? runSpeed : walkSpeed);
    }

    public void AddJumpForce ()
    {
        _rb.AddForce(jumpForce * _rb.mass * Vector3.up);
    }

    // Enables jumping and player movement
    public void EnableMovement ()
    {
        _enableMovement = true;
    }

    // Disables jumping and player movement
    public void DisableMovement ()
    {
        _enableMovement = false;
    }
}
