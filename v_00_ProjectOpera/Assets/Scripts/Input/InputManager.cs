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

    private Controls controls;
    private InputActionMap coreActionMap;
    private InputActionMap UIActionMap;

    public delegate void MoveInputHandler(Vector3 movement);
    public event MoveInputHandler OnMove;

    public delegate void CameraInputHandler(Vector2 movement);
    public event CameraInputHandler OnCamMove;

    public delegate void InteractHandler();
    public event InteractHandler OnInteraction;

    public delegate void PauseHandler();
    public event PauseHandler OnPause;

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

    private void OnEnable()
    {
        EnableCoreControls();
        SceneManager.sceneLoaded += OnSceneLoaded;
        Application.quitting += Quitting;
    }

    private void OnDisable()
    {
        DisableCoreControls();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Application.quitting -= Quitting;
    }

    private void Quitting()
    {
        isQuitting = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == GameManager.Instance.mainGameSceneName)
        {
            EnableCoreControls();
        }
        else if (scene.name == "GameOver")
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
        coreActionMap["Interact"].performed += HandleInteraction;
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

    private void HandleInteraction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            OnInteraction?.Invoke();
        }
    }

    public void HandlePause(InputAction.CallbackContext context)
    {
        if (context.performed)
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
    }

    public void PauseWithButton()
    {
        EnableUIControls();
        DisableCoreControls();
    }
}
