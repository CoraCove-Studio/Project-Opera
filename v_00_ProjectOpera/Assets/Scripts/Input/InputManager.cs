using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private Controls controls;

    // Action maps
    private InputActionMap coreActionMap;
    private InputActionMap UIActionMap;

    public delegate void MoveInputHandler(Vector2 movement);
    public delegate void VertMoveInputHandler(float vertMovement);

    public event MoveInputHandler OnMove;
    // public event VertMoveInputHandler OnVertMove;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            controls = new Controls();
        }
    }

    private void OnEnable()
    {
        controls.Core.Enable();
        controls.Core.Movement.performed += context => HandleMovement(context.ReadValue<Vector2>());
        controls.Core.Movement.canceled += context => HandleMovement(Vector2.zero);
    }

    private void OnDisable()
    {
        controls.Core.Disable();
        controls.Core.Movement.performed -= context => HandleMovement(context.ReadValue<Vector2>());
        controls.Core.Movement.canceled -= context => HandleMovement(Vector2.zero);
    }

    private void HandleMovement(Vector2 movementInput)
    {
        OnMove?.Invoke(movementInput);
    }

}
