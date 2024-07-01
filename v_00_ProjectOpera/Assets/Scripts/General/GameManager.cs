using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Fields and Properties

    public string[] gameScenes = { "MainScene", "emma_MainScene", "rachel_MainScene", "testZeb" };

    public string mainGameSceneName = "MainScene";

    public PlayerUIHandler PlayerUI { get; private set; }
    public GameObject Player { get; private set; }
    private Rigidbody playerRigidBody;
    public AudioManager audioManager;
    private GameTimer gameTimer;
    private float gameDurationInSeconds;

    public bool GamePaused { get; private set; } = true;
    public bool InTutorial { get; private set; } = true;
    private static bool isQuitting = false;
    private static bool currentlyInGameScene = true;

    private const int MinResources = 0;
    private const int MaxResources = 300;
    private const int MaxCredits = 100000;

    private int playerCredits;
    private int playerCrops;
    private int playerParts;
    private int playerNitrogen;

    #region Resources Properties

    public int PlayerCrops
    {
        get { return playerCrops; }
        private set
        {
            playerCrops = Mathf.Clamp(value, MinResources, MaxResources);
        }
    }

    public int PlayerParts
    {
        get { return playerParts; }
        private set
        {
            playerParts = Mathf.Clamp(value, MinResources, MaxResources);
        }
    }

    public int PlayerNitrogen
    {
        get { return playerNitrogen; }
        private set
        {
            playerNitrogen = Mathf.Clamp(value, MinResources, MaxResources);
        }
    }

    public int PlayerCredits
    {
        get { return playerCredits; }
        private set
        {
            playerCredits = Mathf.Clamp(value, MinResources, MaxCredits);
        }
    }

    #endregion

    private readonly Dictionary<string, int> newGameValues = new()
    {
    { "Crops", 5 },
    { "Parts", 5 },
    { "Nitrogen", 5 },
    { "Credits", 300 },
    { "GameDuration", 600 }
};

    #endregion

    #region Singleton pattern

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null && isQuitting != true)
            {
                GameObject gameManager = new("GameManager");
                instance = gameManager.AddComponent<GameManager>();
                DontDestroyOnLoad(gameManager);
                Debug.Log("New GameManager has been created.");
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
#if UNITY_EDITOR 
            // Hacky code to make this work if you play the game from the main scene in editor
            if (gameScenes.Contains(SceneManager.GetActiveScene().name))
            {
                currentlyInGameScene = true;
                SetUpNewGame();
            }
            else
            {
                currentlyInGameScene = false;
            }
#endif
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    private void Update()
    {
        Cheatcodes();
        if (GamePaused == false && currentlyInGameScene)
        {
            PlayerUI.UpdateGameTimer(gameTimer.TimeLeft.Item1, gameTimer.TimeLeft.Item2);
            CheckGameOver();
        }
    }

    #region Unity Event Methods

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InputManager.Instance.OnPause += ToggleGamePause;
        Application.quitting += Quitting;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        InputManager.Instance.OnPause -= ToggleGamePause;
        Application.quitting -= Quitting;
    }

    private void Quitting()
    {
        Debug.Log("Application has begun quitting.");
        isQuitting = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Cursor.lockState = CursorLockMode.Confined;
        print("GameManager: OnSceneLoaded: Current scene is: " + scene.name);
        if (gameScenes.Contains(scene.name))
        {
            currentlyInGameScene = true;
            SetUpNewGame();
        }
        else
        {
            currentlyInGameScene = false;
        }
    }

    #endregion

    public void StartGame()
    {
        InTutorial = false;
        // Called when the player exits the tutorial using the "Start" button
        ToggleGamePause();
        gameTimer.StartTimer();
        // TODO: move the player to new location here so they don't get launched
        playerRigidBody.isKinematic = false;
    }
    private void SetUpNewGame()
    {
        Debug.Log("GameManager: SetUpNewGame: Setting up new game.");

        FindImportantReferences();
        SetNewGameValues();
        PlayerUI.UpdateUI();

        GamePaused = true;
        InTutorial = true;

        gameTimer.SetNewTimer(gameDurationInSeconds);
        PlayerUI.ToggleReticleVisibility(false);
        Time.timeScale = 1;
    }
    private void SetNewGameValues()
    {
        playerCrops = newGameValues["Crops"];
        playerParts = newGameValues["Parts"];
        playerNitrogen = newGameValues["Nitrogen"];
        playerCredits = newGameValues["Credits"];
        gameDurationInSeconds = newGameValues["GameDuration"];
        Debug.Log("GameManager: SetNewGameValues: " + gameDurationInSeconds);
    }

    public void DisplayTooltip(int value)
    {
        PlayerUI.DisplayTooltip(value);
    }
    public void DisplayTooltip(ResourceTypes resourceType, int value)
    {
        PlayerUI.DisplayTooltip(resourceType, value);
    }

    public void ClearTooltipDisplay()
    {
        PlayerUI.tooltipHandler.ClearDisplay();
    }

    private void FindImportantReferences()
    {
        PlayerUI = GameObject.Find("PlayerCanvas").GetComponent<PlayerUIHandler>();
        if (PlayerUI == null) Debug.Log("GameManager: FindImportantReferences: PlayerUI not found.");

        GameObject.Find("GameTimer").TryGetComponent(out gameTimer);
        if (gameTimer == null) Debug.Log("GameManager: FindImportantReferences: gameTimer not found.");

        GameObject.Find("PlayerPrefab").TryGetComponent(out playerRigidBody);
        if (playerRigidBody == null) Debug.Log("GameManager: FindImportantReferences: playerRigidBody not found.");

        GameObject.Find("AudioManager").TryGetComponent(out audioManager);
        if (audioManager == null) Debug.Log("GameManager: FindImportantReferences: audioManager not found.");
    }

    private void CheckGameOver()
    {
        if (gameTimer != null && gameTimer.CountdownTimer <= 0)
        {
            if (GamePaused) // ensure that the game isn't paused to avoid weird bugs
            {
                ToggleGamePause();
            }
            SceneManager.LoadScene("GameOver");
        }
    }

    public void ToggleGamePause()
    {
        // This method is subscribed to the OnPause event from the InputHandler and 
        // is called whenever the player presses `esc`
        Scene scene = SceneManager.GetActiveScene();
        if (gameScenes.Contains(scene.name) && InTutorial == false)
        {
            if (GamePaused == true)
            {
                Debug.Log("GameManager: ToggleGamePause: Resuming game.");
                Time.timeScale = 1;
                AudioListener.pause = false;
                if (PlayerUI != null)
                {
                    PlayerUI.ResumeGame();
                }
                GamePaused = false;
            }
            else
            {
                Debug.Log("GameManager: ToggleGamePause: Pausing game.");
                Time.timeScale = 0;
                AudioListener.pause = true;
                if (PlayerUI != null)
                {
                    PlayerUI.PauseGame();
                }
                GamePaused = true;
            }
        }
        else
        {
            Debug.Log("GameManager: ToggleGamePause: Can't toggle.");
        }
    }

    public void ActivateSubMenu()
    {
        GamePaused = true;
    }

    #region Add / Take Resource Methods

    public bool CheckPlayerResourceValue(int value, ResourceTypes resourceType)
    {
        switch (resourceType)
        {
            case ResourceTypes.CROP:
                return PlayerCrops >= value;
            case ResourceTypes.PART:
                return PlayerParts >= value;
            case ResourceTypes.NITROGEN:
                return PlayerNitrogen >= value;
            default:
                print("GameManager: GetPlayerResourceValue: Getting resource value failed, not recognized resource type.");
                return false;
        }
    }

    public void AddResourceToPlayer(int amount, ResourceTypes resourceType)
    {
        switch (resourceType)
        {
            case ResourceTypes.CROP:
                PlayerCrops += amount;
                Debug.Log($"GameManager: AddResourceToPlayer: Adding {amount} to PlayerCrops. Current value is {PlayerCrops}");
                break;
            case ResourceTypes.PART:
                PlayerParts += amount;
                Debug.Log($"GameManager: AddResourceToPlayer: Adding {amount} to PlayerParts. Current value is {PlayerParts}");
                break;
            case ResourceTypes.NITROGEN:
                PlayerNitrogen += amount;
                Debug.Log($"GameManager: AddResourceToPlayer: Adding {amount} to PlayerNitrogen. Current value is {PlayerNitrogen}");
                break;
            default:
                print("GameManager: AddResourceToPlayer: Adding resource failed, not recognized resource type.");
                break;
        }
        PlayerUI.UpdateUI();
    }

    public void TakeResourceFromPlayer(int amount, ResourceTypes resourceType)
    {
        switch (resourceType)
        {
            case ResourceTypes.CROP:
                PlayerCrops -= amount;
                Debug.Log($"GameManager: TakeResourceFromPlayer: Taking {amount} from PlayerCrops. Current value is {PlayerCrops}");
                break;
            case ResourceTypes.PART:
                PlayerParts -= amount;
                Debug.Log($"GameManager: TakeResourceFromPlayer: Taking {amount} from PlayerParts. Current value is {PlayerParts}");
                break;
            case ResourceTypes.NITROGEN:
                PlayerNitrogen -= amount;
                Debug.Log($"GameManager: TakeResourceFromPlayer: Taking {amount} from PlayerNitrogen. Current value is {PlayerNitrogen}");
                break;
            default:
                print("Taking resource failed, not recognized resource type.");
                break;
        }
        PlayerUI.UpdateUI();
    }
    public void AddCreditsToPlayer(int amount)
    {
        PlayerCredits += amount;
        Debug.Log($"GameManager: AddCreditsToPlayer: Adding {amount} to PlayerCredits. Current value is {PlayerCredits}");
        PlayerUI.UpdateUI();
    }

    public void TakeCreditsFromPlayer(int amount)
    {
        PlayerCredits -= amount;
        Debug.Log($"GameManager: TakeCreditsToPlayer: Taking {amount} from PlayerCredits. Current value is {PlayerCredits}");
        PlayerUI.UpdateUI();
    }


    #endregion

    #region Cheatcodes

    private void Cheatcodes()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            AddResourceToPlayer(1, ResourceTypes.CROP);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            AddResourceToPlayer(1, ResourceTypes.PART);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddResourceToPlayer(1, ResourceTypes.NITROGEN);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddCreditsToPlayer(1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            // restart outputInterval
        }
        if (Input.GetKeyDown(KeyCode.P)) // Debugging for other scenes
        {
            SetUpNewGame();
        }
    }

    #endregion
}
