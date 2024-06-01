using System.Runtime.CompilerServices;
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
    // public delegate void InteractInputHandler()

    public event MoveInputHandler OnMove;
    public event VertMoveInputHandler OnVertMove;
    //public event InteractInputHandler 

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
        controls.Core.Movement.performed += context => HandleVertMovement(context.ReadValue<float>());
        controls.Core.Movement.canceled += context => HandleVertMovement(0);
    }

    private void OnDisable()
    {
        controls.Core.Disable();
        controls.Core.Movement.performed -= context => HandleMovement(context.ReadValue<Vector2>());
        controls.Core.Movement.canceled -= context => HandleMovement(Vector2.zero);
        controls.Core.Movement.performed -= context => HandleVertMovement(context.ReadValue<float>());
        controls.Core.Movement.canceled -= context => HandleVertMovement(0);
    }

    private void HandleMovement(Vector2 movementInput)
    {
        OnMove?.Invoke(movementInput);
    }

    private void HandleVertMovement(float vertMovementInput)
    {
        OnVertMove?.Invoke(vertMovementInput);
    }

}
