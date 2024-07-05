using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float xSensitivity = 0.5f;
    [SerializeField] private float ySensitivity = 0.5f;
    [SerializeField] private float maxPitchAngle = 85f;  // Max angle for looking up and down
    [SerializeField] private float smoothing = 5f; // Smoothing factor

    private Vector2 accumulatedInput;
    private Vector3 currentMovementInput;
    private Quaternion currentRotation;
    private Quaternion targetRotation;

    private readonly WaitForFixedUpdate _waitForFixedUpdate = new();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(LateFixedUpdate());
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMove += ReceiveMovementInput;
            InputManager.Instance.OnCamMove += ReceiveCamInput;
        }
        else
        {
            Debug.Log("No input manager found.");
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMove -= ReceiveMovementInput;
            InputManager.Instance.OnCamMove -= ReceiveCamInput;
        }
    }

    private void Start()
    {
        // Initialize the current and target rotation to the current rotation of the camera
        currentRotation = transform.rotation;
        targetRotation = currentRotation;
    }

    private void Update()
    {
        AccumulateInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void LateUpdate()
    {
        HandleCamera();
    }

    private IEnumerator LateFixedUpdate()
    {
        while (true)
        {
            yield return _waitForFixedUpdate;
            HandleCamera();
        }
    }

    private void HandleMovement()
    {
        if (currentMovementInput.sqrMagnitude > 0.001f)
        {
            // Construct the input vector correctly
            Vector3 inputVector = transform.forward * currentMovementInput.z +
                                  transform.up * currentMovementInput.y +
                                  transform.right * currentMovementInput.x;

            // Calculate the force to be added based on the input direction and acceleration
            Vector3 force = acceleration * inputVector;

            // Apply the force to the rigidbody
            rb.AddForce(force, ForceMode.Acceleration);
        }
        // Note: Rigidbody's drag property will handle this over time, no need to manually apply drag
    }

    private void AccumulateInput()
    {
        // Have to flip the x and y from the input
        float inputX = accumulatedInput.y * xSensitivity;
        float inputY = accumulatedInput.x * ySensitivity;

        // Calculate the new rotation while clamping the z rotation to 0
        targetRotation *= Quaternion.Euler(-inputX * lookSpeed * Time.deltaTime, inputY * lookSpeed * Time.deltaTime, 0);

        // Convert targetRotation.eulerAngles.x to the range of -180 to 180 before clamping
        Vector3 targetEulerAngles = targetRotation.eulerAngles;
        if (targetEulerAngles.x > 180) targetEulerAngles.x -= 360;

        // Clamp the x rotation to the specified min and max pitch angles
        targetEulerAngles.x = Mathf.Clamp(targetEulerAngles.x, -maxPitchAngle, maxPitchAngle);

        // Clamp the z rotation to 0
        targetEulerAngles.z = 0;

        // Apply the clamped rotation back to targetRotation
        targetRotation = Quaternion.Euler(targetEulerAngles);

    }

    private void HandleCamera()
    {
        // Smoothly interpolate between the current rotation and the target rotation
        Quaternion interpolatedRotation = Quaternion.Slerp(currentRotation, targetRotation, smoothing * Time.deltaTime);

        // Convert the interpolated rotation to Euler angles
        Vector3 interpolatedEulerAngles = interpolatedRotation.eulerAngles;

        // Clamp the z rotation to 0
        interpolatedEulerAngles.z = 0;

        // Apply the clamped rotation back to the transform
        transform.localRotation = Quaternion.Euler(interpolatedEulerAngles);

        // Update the current rotation
        currentRotation = transform.localRotation;

        // Reset accumulated input
        accumulatedInput = Vector2.zero;
    }

    private void ReceiveMovementInput(Vector3 movementInput)
    {
        currentMovementInput = movementInput;
    }

    public void ReceiveCamInput(Vector2 input)
    {
        accumulatedInput += input;
    }
}
