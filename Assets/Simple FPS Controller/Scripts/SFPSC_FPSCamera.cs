using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class SFPSC_FPSCamera : MonoBehaviour
{
    private Camera _cam;
    
    public float sensitivity = 0.2f;
    [HideInInspector]
    private Vector2 mouseMovement;
    public float maxUpAngle = 80;
    public float maxDownAngle = -80;
    public Transform player;
    public Transform CameraPosition;

    [Header("Input")]
    public InputActionReference mouseMoveAction;

    private void Awake()
    {
        _cam = this.GetComponent<Camera>();

        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }
    
    private float rotX = 0.0f, rotY = 0.0f;
    [HideInInspector]
    public float rotZ = 0.0f;
    private void Update()
    {
        // Mouse input
        mouseMovement = mouseMoveAction.action.ReadValue<Vector2>() * sensitivity;

        // Calculations
        rotX -= mouseMovement.y;
        rotX = Mathf.Clamp(rotX, maxDownAngle, maxUpAngle);
        rotY += mouseMovement.x;

        // Placing values
        transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
        player.Rotate(Vector3.up * mouseMovement.x);
        transform.position = CameraPosition.position;
    }

    public void Shake(float magnitude, float duration)
    {
        StartCoroutine(IShake(magnitude, duration));
    }

    private IEnumerator IShake(float mag, float dur)
    {
        WaitForEndOfFrame wfeof = new WaitForEndOfFrame();
        for(float t = 0.0f; t <= dur; t += Time.deltaTime)
        {
            rotZ = Random.Range(-mag, mag) * (t / dur - 1.0f);
            yield return wfeof;
        }
        rotZ = 0.0f;
    }
}
