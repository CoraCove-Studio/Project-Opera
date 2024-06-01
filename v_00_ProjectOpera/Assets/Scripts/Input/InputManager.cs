using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private Controls controls;

    public delegate void MoveInputHandler(Vector2 movement);
    public event MoveInputHandler OnMove;

    public delegate void VertMoveInputHandler(float vertMovement);
    public event VertMoveInputHandler OnVertMove;

    public delegate void CameraInputHandler(Vector2 movement);
    public event CameraInputHandler OnCamMove;

    // Action maps
    private InputActionMap coreActionMap;
    private InputActionMap UIActionMap;



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
        controls.Core.Movement.performed += HandleMovement;
        controls.Core.VerticalMove.performed += HandleVertMovement;
        controls.Core.Look.performed += HandleCamMovement;
    }

    private void OnDisable()
    {
        controls.Core.Disable();
        controls.Core.Movement.performed -= HandleMovement;
        controls.Core.VerticalMove.performed -= HandleVertMovement;
        controls.Core.Look.performed -= HandleCamMovement;

    }

    public void HandleMovement(InputAction.CallbackContext ctx)
    {
        Vector2 movementInput = ctx.ReadValue<Vector2>();
        OnMove?.Invoke(movementInput);
    }

    public void HandleVertMovement(InputAction.CallbackContext ctx)
    {
        float vertMovement = ctx.ReadValue<float>();
        OnVertMove?.Invoke(vertMovement);
    }

    public void HandleCamMovement(InputAction.CallbackContext ctx)
    {
        Vector2 movementInput = ctx.ReadValue<Vector2>();
        OnCamMove?.Invoke(movementInput);
    }

}
