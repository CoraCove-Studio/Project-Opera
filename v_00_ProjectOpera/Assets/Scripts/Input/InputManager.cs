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

            if (FindAndAssignInputActionAsset())
            {
                LoadSavedBindings();
                InitializeControls();
            }
            else
            {
                Debug.LogError("Failed to find and assign InputActionAsset. InputManager initialization aborted.");
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private InputActionAsset inputActions;
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
        //inputActions = InputActionAsset.FromJson()
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Application.quitting -= Quitting;
    }

    private bool FindAndAssignInputActionAsset()
    {
        GameObject playerCanvas = GameObject.Find("PlayerCanvas");
        if (playerCanvas == null)
        {
            Debug.LogError("PlayerCanvas GameObject not found in the scene!");
            return false;
        }

        SettingsMenu settingsMenu = playerCanvas.GetComponentInChildren<SettingsMenu>();
        if (settingsMenu == null)
        {
            Debug.LogError("SettingsMenu component not found on PlayerCanvas!");
            return false;
        }

        inputActions = settingsMenu.GetInputActions();
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset not found in SettingsMenu!");
            return false;
        }

        Debug.Log("InputActionAsset successfully assigned from SettingsMenu.");
        return true;
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
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset is not assigned to InputManager!");
            return;
        }

        coreActionMap = inputActions.FindActionMap("Core");
        UIActionMap = inputActions.FindActionMap("UI");

        if (coreActionMap == null || UIActionMap == null)
        {
            Debug.LogError("Core or UI action map not found in the InputActionAsset!");
            return;
        }

        // Assign action handlers
        coreActionMap.FindAction("Movement").performed += HandleMovement;
        coreActionMap.FindAction("Movement").canceled += HandleMovement;
        coreActionMap.FindAction("Look").performed += HandleCamMovement;
        coreActionMap.FindAction("Interact").started += StartInteraction;
        coreActionMap.FindAction("Interact").canceled += StopInteraction;
        coreActionMap.FindAction("Pause").performed += HandlePause;

        UIActionMap.FindAction("Pause").performed += HandlePause;
    }

    public void LoadSavedBindings()
    {
        string rebinds = PlayerPrefs.GetString("InputBindings", string.Empty);
        if (!string.IsNullOrEmpty(rebinds))
        {
            inputActions.LoadBindingOverridesFromJson(rebinds);
        }
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
