using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float xSensitivity = .5f;
    [SerializeField] private float ySensitivity = .5f;
    [SerializeField] private float maxPitchAngle = 85f;  // Max angle for looking up and down

    private Vector3 currentMovementInput;
    private bool movementInputGiven = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnMove += ReceiveMovementInput;
        InputManager.Instance.OnCamMove += ReceiveCamInput;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMove -= ReceiveMovementInput;
        InputManager.Instance.OnCamMove -= ReceiveCamInput;
    }

    private void FixedUpdate()
    {
        // OutputDebugLog();
        HandleMovement();
    }

    private void HandleMovement()
    {
        movementInputGiven = currentMovementInput.magnitude > 0.001f;
        if (movementInputGiven)
        {
            // Construct the input vector correctly
            Vector3 inputVector = transform.forward * currentMovementInput.z + transform.up * currentMovementInput.y + transform.right * currentMovementInput.x;

            // Compute the move vector using the input vector
            Vector3 moveVector = movementSpeed * Time.fixedDeltaTime * inputVector;

            // Move the rigidbody to the new position
            rb.MovePosition(rb.position + moveVector);
        }
    }

    private void HandleCamera(Vector2 input)
    {
            // Have to flip the x and y from the input
            float inputX = input.y * xSensitivity;
            float inputY = input.x * ySensitivity;

            Vector3 currentRotation = transform.localEulerAngles;

            // Calculate the new rotation while clamping the z rotation to 0
            currentRotation.x -= inputX * Time.deltaTime * lookSpeed;
            currentRotation.y += inputY * Time.deltaTime * lookSpeed;

            // Convert currentRotation.x to the range of -180 to 180 before clamping
            if (currentRotation.x > 180) currentRotation.x -= 360;

            // Clamp the x rotation to the specified min and max pitch angles
            currentRotation.x = Mathf.Clamp(currentRotation.x, -maxPitchAngle, maxPitchAngle);

            currentRotation.z = 0;

            transform.localEulerAngles = currentRotation;
    }

    private void OutputDebugLog()
    {
        Debug.ClearDeveloperConsole();
        print("Is movement input given?: " + movementInputGiven);
        print("Current movement input vector: " + currentMovementInput);
    }

    private void ReceiveMovementInput(Vector3 movementInput)
    {
        currentMovementInput = movementInput;
    }

    private void ReceiveCamInput(Vector2 input)
    {
        HandleCamera(input);
    }
}
