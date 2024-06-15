using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float drag = 2f;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float xSensitivity = 0.5f;
    [SerializeField] private float ySensitivity = 0.5f;
    [SerializeField] private float maxPitchAngle = 85f;  // Max angle for looking up and down
    [SerializeField] private float smoothing = 5f; // Smoothing factor
    private Vector2 accumulatedInput;

    private Vector3 currentMovementInput;
    private Vector3 currentVelocity;
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
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
            print("No input manager found.");
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
        currentRotation = transform.localEulerAngles;
        targetRotation = currentRotation;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void LateUpdate()
    {
        HandleCamera();
    }

    private void HandleMovement()
    {
        if (currentMovementInput.sqrMagnitude > 0.001f)
        {
            // Construct the input vector correctly
            Vector3 inputVector = transform.forward * currentMovementInput.z +
                                  transform.up * currentMovementInput.y +
                                  transform.right * currentMovementInput.x;

            // Accelerate the current velocity towards the input direction
            currentVelocity += acceleration * Time.fixedDeltaTime * inputVector;

            // Clamp the velocity to the maximum movement speed
            if (currentVelocity.magnitude > movementSpeed)
            {
                currentVelocity = currentVelocity.normalized * movementSpeed;
            }
        }

        // Apply drag to the current velocity to simulate gradual slowing down
        currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, drag * Time.fixedDeltaTime);

        // Move the rigidbody using the updated velocity
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    private void HandleCamera()
    {
        // Have to flip the x and y from the input
        float inputX = accumulatedInput.y * xSensitivity;
        float inputY = accumulatedInput.x * ySensitivity;

        // Calculate the new rotation while clamping the z rotation to 0
        targetRotation.x -= inputX * Time.deltaTime * lookSpeed;
        targetRotation.y += inputY * Time.deltaTime * lookSpeed;

        // Convert targetRotation.x to the range of -180 to 180 before clamping
        if (targetRotation.x > 180) targetRotation.x -= 360;

        // Clamp the x rotation to the specified min and max pitch angles
        targetRotation.x = Mathf.Clamp(targetRotation.x, -maxPitchAngle, maxPitchAngle);

        targetRotation.z = 0;

        // Smoothly interpolate between the current rotation and the target rotation
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, smoothing * Time.deltaTime);

        transform.localEulerAngles = currentRotation;

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
