using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Camera playerCam;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float verticalSpeed = 3f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float xSensitivity = .5f;
    [SerializeField] private float ySensitivity = .5f;
    [SerializeField] private float maxPitchAngle = 89f;  // Max angle for looking up and down
    [SerializeField] private float xRotation = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnMove += HandleRegularMovement;
        InputManager.Instance.OnVertMove += HandleVertMovement;
        InputManager.Instance.OnCamMove += HandleCameraMovement;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMove -= HandleRegularMovement;
        InputManager.Instance.OnVertMove -= HandleVertMovement;
        InputManager.Instance.OnCamMove -= HandleCameraMovement;
    }

    private void HandleRegularMovement(Vector2 movementInput)
    {

    }

    private void HandleVertMovement(float vertMovement)
    {

    }

    private void HandleCameraMovement(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        // calculating the camera rotation for looking up and down
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -maxPitchAngle, maxPitchAngle);

        // this applies to the cameras transform
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // rotating the camera to look left and right
        transform.Rotate((mouseX * Time.deltaTime) * xSensitivity * Vector3.up);
    }
}
