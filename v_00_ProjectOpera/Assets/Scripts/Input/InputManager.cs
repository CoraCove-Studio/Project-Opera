using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject InputManager = new("InputManager");
                instance = InputManager.AddComponent<InputManager>();
                DontDestroyOnLoad(InputManager);
            }
            return instance;
        }
    }
    private Controls controls;

    public delegate void MoveInputHandler(Vector3 movement);
    public event MoveInputHandler OnMove;

    public delegate void CameraInputHandler(Vector2 movement);
    public event CameraInputHandler OnCamMove;

    public delegate void InteractHandler();
    public event InteractHandler OnInteraction;

    public delegate void PauseHandler();
    public event PauseHandler OnPause;

    // Action maps
    private InputActionMap coreActionMap;
    private InputActionMap UIActionMap;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            controls = new Controls();
            coreActionMap = controls.Core;
            UIActionMap = controls.UI;
            print("Action maps loaded.");
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            print("Additional InputManager created and destroyed.");
        }
    }

    private void OnEnable()
    {
        EnableCoreControls();
    }

    private void OnDisable()
    {
        DisableCoreControls();
    }

    public void HandleMovement(InputAction.CallbackContext ctx)
    { 
        OnMove?.Invoke(ctx.ReadValue<Vector3>());
    }

    public void HandleCamMovement(InputAction.CallbackContext ctx)
    {
        Vector2 movementInput = ctx.ReadValue<Vector2>();
        OnCamMove?.Invoke(movementInput);
    }

    public void HandleInteraction(InputAction.CallbackContext ctx)
    {
        OnInteraction?.Invoke();
    }

    private void EnableCoreControls()
    {
        coreActionMap.Enable();
        coreActionMap["Movement"].performed += HandleMovement;
        coreActionMap["Movement"].canceled += HandleMovement;
        coreActionMap["Look"].performed += HandleCamMovement;
        coreActionMap["Interact"].performed += HandleInteraction;
        coreActionMap["Pause"].performed += HandlePause; // Ensure Pause action is set up
    }

    private void DisableCoreControls()
    {
        coreActionMap.Disable();
        coreActionMap["Movement"].performed -= HandleMovement;
        coreActionMap["Movement"].canceled -= HandleMovement;
        coreActionMap["Look"].performed -= HandleCamMovement;
        coreActionMap["Interact"].performed -= HandleInteraction;
        coreActionMap["Pause"].performed -= HandlePause;
    }

    private void EnableUIControls()
    {
        UIActionMap.Enable();
        UIActionMap["Pause"].performed += HandlePause;
    }

    private void DisableUIControls()
    {
        UIActionMap.Disable();
        UIActionMap["Pause"].performed -= HandlePause;
    }

    private void HandlePause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (coreActionMap.enabled)
            {
                print("Pause activated.");
                DisableCoreControls();
                EnableUIControls();
                OnPause?.Invoke();
            }
            else
            {
                print("Pause deactivated.");
                DisableUIControls();
                EnableCoreControls();
                OnPause?.Invoke();
            }
        }
    }
}
