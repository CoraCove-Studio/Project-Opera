using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    private static bool isQuitting = false;
    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (instance == null && isQuitting != true)
            {
                GameObject inputManager = new("InputManager");
                instance = inputManager.AddComponent<InputManager>();
                DontDestroyOnLoad(inputManager);
                Debug.Log("New InputManager has been created.");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeControls();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private Controls controls;
    private InputActionMap coreActionMap;
    private InputActionMap UIActionMap;

    public delegate void MoveInputHandler(Vector3 movement);
    public event MoveInputHandler OnMove;

    public delegate void CameraInputHandler(Vector2 movement);
    public event CameraInputHandler OnCamMove;

    public delegate void InteractHandler();
    public event InteractHandler OnInteractionStart;
    public event InteractHandler OnInteractionStop;

    public delegate void PauseHandler();
    public event PauseHandler OnPause;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Application.quitting += Quitting;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Application.quitting -= Quitting;
    }

    private void Quitting()
    {
        isQuitting = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance.gameScenes.Contains(scene.name))
        {
            EnableUIControls();
        }
        else
        {
            DisableCoreControls();
            EnableUIControls();
        }
    }

    private void InitializeControls()
    {
        controls = new Controls();
        coreActionMap = controls.Core;
        UIActionMap = controls.UI;

        // Assign action handlers
        coreActionMap["Movement"].performed += HandleMovement;
        coreActionMap["Movement"].canceled += HandleMovement;
        coreActionMap["Look"].performed += HandleCamMovement;
        coreActionMap["Interact"].started += StartInteraction;
        coreActionMap["Interact"].canceled += StopInteraction;
        coreActionMap["Pause"].performed += HandlePause;

        UIActionMap["Pause"].performed += HandlePause;
    }

    private void EnableCoreControls()
    {
        coreActionMap.Enable();
    }

    private void DisableCoreControls()
    {
        coreActionMap.Disable();
    }

    private void EnableUIControls()
    {
        UIActionMap.Enable();
    }

    private void DisableUIControls()
    {
        UIActionMap.Disable();
    }

    private void HandleMovement(InputAction.CallbackContext ctx)
    {
        OnMove?.Invoke(ctx.ReadValue<Vector3>());
    }

    private void HandleCamMovement(InputAction.CallbackContext ctx)
    {
        OnCamMove?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void StartInteraction(InputAction.CallbackContext ctx)
    {
        OnInteractionStart?.Invoke();
    }

    private void StopInteraction(InputAction.CallbackContext ctx)
    {
        OnInteractionStop?.Invoke();
    }

    private void HandlePause(InputAction.CallbackContext context)
    {
        if (context.performed && GameManager.Instance.InTutorialMonitor == false)
        {
            if (coreActionMap.enabled)
            {
                DisableCoreControls();
                EnableUIControls();
                Debug.Log("InputManager: OnPause: Core disabled, UI enabled.");
            }
            else
            {
                DisableUIControls();
                EnableCoreControls();
                Debug.Log("InputManager: OnPause: Core enabled, UI disabled.");
            }
            OnPause?.Invoke();
        }
    }

    public void UnpauseWithButton()
    {
        DisableUIControls();
        EnableCoreControls();
        Debug.Log("InputManager: UnPauseWithButton: Core enabled, UI disabled.");
    }

    public void PauseWithButton()
    {
        EnableUIControls();
        DisableCoreControls();
        Debug.Log("InputManager: PauseWithButton: Core disabled, UI enabled.");
    }
}
