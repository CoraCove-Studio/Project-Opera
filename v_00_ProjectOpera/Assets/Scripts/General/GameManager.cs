using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Fields and Properties

    public string[] gameScenes = { "MainScene", "emma_MainScene", "rachel_MainScene", "Tutorial" };

    public string mainGameSceneName = "MainScene";

    public PlayerUIHandler PlayerUI { get; private set; }
    public GameObject Player { get; private set; }
    private Rigidbody playerRigidBody;
    public AudioManager audioManager;
    private GameTimer gameTimer;
    public PlanetRotation PlanetRotation { get; private set; }
    public TutorialHandler TutorialHandler { get; private set; }
    public DebtInterface DebtUI { get; private set; }
    public SceneTransition SceneTransition { get; private set; }
    public SettingsMenu SettingsMenu { get; private set; }

    private float gameDurationInSeconds;
    public bool GamePaused { get; private set; } = true;
    public bool InTutorial { get; private set; } = false;
    public bool InTutorialMonitor { get; private set; } = true;
    private static bool isQuitting = false;
    private static bool currentlyInGameScene = true;
    public static bool gameJustStarted = true;

    private const int MinResources = 0;
    private const int MaxResources = 5000;
    private const int MaxCredits = 10000000;
    private const int MaxDebt = 35000;

    private int playerCredits;
    private int playerCrops;
    private int playerParts;
    private int playerNitrogen;
    private int playerDebt;

    public Dictionary<string, object> SettingsDictionary { get; private set; }



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
    { "GameDuration", 600 },
    { "PlayerDebt", 35000 }
};

    #endregion

    #region Statistics And Debt

    public Dictionary<string, int> playerStatistics = new()
    {
        { "Player Debt", 35000 },   //updated via void method
        { "Net Profit", 0 },        //updated via void method
        { "Credits Earned", 0 },    //updated in game manager
        { "Credits Spent", 0 },     //updated in game manager
        { "Machines Broken", 0 },   //updated via void method
        { "Items Produced", 0 },    //updated via void method
        { "Items Collected", 0 }    //updated in game manager
    };
    
    public int PlayerDebt
    {
        get { return playerDebt; }
        private set
        {
            playerDebt = Mathf.Clamp(value, MinResources, MaxDebt);
        }
    }
    public void PayDebt(int amount)
    {
        PlayerDebt -= amount;
        playerStatistics["Player Debt"] -= amount;
        DebtUI.UpdateStatistics(playerStatistics);
        UpdateSuccessfulRuns();
    }

    public void UpdateNetProfit()
    {
        playerStatistics["Net Profit"] = playerStatistics["Credits Earned"] - playerStatistics["Credits Spent"];
        DebtUI.UpdateStatistics(playerStatistics);
        UpdateHighScore();
    }

    public void UpdateMachinesBroken()
    {
        playerStatistics["Machines Broken"]++;
        DebtUI.UpdateStatistics(playerStatistics);
    }

    public void UpdateItemsProduced()
    {
        playerStatistics["Items Produced"]++;
        DebtUI.UpdateStatistics(playerStatistics);
    }

    private void ResetPlayerStatistics()
    {
        playerStatistics["Player Debt"] = MaxDebt;
        playerStatistics["Net Profit"] = 0;
        playerStatistics["Credits Earned"] = 0;
        playerStatistics["Credits Spent"] = 0;
        playerStatistics["Machines Broken"] = 0;
        playerStatistics["Items Produced"] = 0;
        playerStatistics["Items Collected"] = 0;
    }
    #endregion

    #region Settings Methods

    public void UpdateSettingsDictionary(Dictionary<string, object> settingsDict)
    {
        SettingsDictionary = settingsDict;
        if (currentlyInGameScene) audioManager.SetAudioMixerValues((float)SettingsDictionary["MusicVolume"], (float)SettingsDictionary["SFXVolume"]);
    }

    #endregion

    #region High Score Properties and Methods

    [SerializeField] private int highScore;
    [SerializeField] private int successfulRunsCount;
    public int HighScore
    {
        get => highScore;
        private set => highScore = value;
    }
    public int SuccessfulRunsCount
    {
        get => successfulRunsCount;
        private set => successfulRunsCount = value;
    }

    public void LoadGameData()
    {
        HighScore = PlayerPrefs.GetInt("highScore");
        SuccessfulRunsCount = PlayerPrefs.GetInt("successfulRunsCount");
    }

    public void ResetSuccessfulRuns()
    {
        PlayerPrefs.SetInt("successfulRunsCount", 0);
        PlayerPrefs.Save();
        SuccessfulRunsCount = 0;
    }

    public void SaveGameData()
    {
        PlayerPrefs.SetInt("highScore", HighScore);
        PlayerPrefs.SetInt("successfulRunsCount", SuccessfulRunsCount);
        PlayerPrefs.Save();
    }

    private void UpdateHighScore()
    {
        if (playerStatistics["Net Profit"] > HighScore)
        {
            HighScore = playerStatistics["Net Profit"];
            Debug.Log($"HighScore updated to {HighScore}");
        }
    }

    public void UpdateSuccessfulRuns()
    {
        if(PlayerDebt <= 0)
        {
            SuccessfulRunsCount++;
            Debug.Log($"SuccessfulRunsCount updated to {SuccessfulRunsCount}");
        }
    }

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
        if (GamePaused == false && currentlyInGameScene && InTutorial == false)
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

    private void LoadPlayerPrefs()
    {

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("GameManager: OnSceneLoaded: Current scene is: " + scene.name);
        if (gameScenes.Contains(scene.name))
        {
            currentlyInGameScene = true;
            SetUpNewGame();
        }
        else
        {
            currentlyInGameScene = false;
        }
        if (scene.name == "MainMenu")
        {
            AudioListener.pause = false;
            InTutorial = false;
            InTutorialMonitor = true;
            GamePaused = true;
        }
        SceneTransition = GameObject.Find("SceneTransition").GetComponent<SceneTransition>();
        if (SceneTransition == null) Debug.Log("GameManager: FindImportantReferences: SceneTransition not found.");
    }

    #endregion

    #region Start Game Methods
    public void StartGame()
    {
        InTutorial = false;
        SetUpPlayerValuesAndUI(InTutorial);
        // Called when the player exits the tutorial using the "Start" button
        if (GamePaused) ToggleGamePause();
        gameTimer.StartTimer();
        playerRigidBody.isKinematic = false;
        PlanetRotation.StartRotationCoroutine();
        Debug.Log("GameManager: StartGame: Game started in normal mode.");
    }

    public void StartTutorial()
    {
        InTutorial = true;
        SetUpPlayerValuesAndUI(InTutorial);
        if (GamePaused) ToggleGamePause();
        playerRigidBody.isKinematic = false;
        Debug.Log("GameManager: StartTutorial: Game started in tutorial mode.");
        // other things going to happen

        TutorialHandler.StartTutorialCoroutine();
    }

    public void StartGameFromTutorial()
    {
        SetTutorialMonitor(false);
        gameTimer.StartTimer();
        PlanetRotation.StartRotationCoroutine();
        Debug.Log("GameManager: StartGameFromTutorial: Starting a game from the tutorial.");
    }
    private void SetUpNewGame()
    {
        Debug.Log("GameManager: SetUpNewGame: Setting up new game.");

        FindImportantReferences();
        GamePaused = true;
        PlayerUI.ToggleReticleVisibility(false);
        Time.timeScale = 1;
    }

    private void SetUpPlayerValuesAndUI(bool inTutorial)
    {
        SetNewGameValues(inTutorial);
        gameTimer.SetNewTimer(gameDurationInSeconds);
        PlayerUI.UpdateUI();
        ResetPlayerStatistics();
        DebtUI.UpdateStatistics(playerStatistics);

    }
    private void SetNewGameValues(bool inTutorial)
    {
        if (inTutorial == true)
        {
            playerCrops = 1;
            playerParts = 0;
            playerNitrogen = 0;
            Debug.Log("GameManager: SetNewGameValues: Set new game values for normal game.");
        }
        else
        {
            playerCrops = newGameValues["Crops"];
            playerParts = newGameValues["Parts"];
            playerNitrogen = newGameValues["Nitrogen"];
            Debug.Log("GameManager: SetNewGameValues: Set new game values for normal game.");
        }
        playerCredits = newGameValues["Credits"];
        gameDurationInSeconds = newGameValues["GameDuration"];
        playerDebt = newGameValues["PlayerDebt"];
    }
    #endregion

    #region Tooltip Methods
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

    #endregion

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

        DebtUI = GameObject.Find("DebtUI").GetComponent<DebtInterface>();
        if (DebtUI == null) Debug.Log("GameManager: FindImportantReferences: DebtUI not found.");

        TutorialHandler = GameObject.Find("TutorialHandler").GetComponent<TutorialHandler>();
        if (TutorialHandler == null) Debug.Log("GameManager: FindImportantReferences: TutorialHandler not found.");

        PlanetRotation = GameObject.Find("PlanetRotation").GetComponent<PlanetRotation>();
        if (PlanetRotation == null) Debug.Log("GameManager: FindImportantReferences: PlanetRotation not found.");
    }

    #region Game Over Methods
    private void CheckGameOver()
    {
        if (gameTimer != null && gameTimer.CountdownTimer <= 0)
        {
            if (GamePaused) // ensure that the game isn't paused to avoid weird bugs
            {
                ToggleGamePause();
            }
            {
                UpdateHighScore();
                SaveGameData();
                SceneTransition.StartSceneClose();
                Invoke(nameof(LoadEndScene), 1f);
            }
        }
    }

    private void LoadEndScene()
    {
        SceneManager.LoadScene("GameOver");
    }
    #endregion

    #region Tutorial Methods
    public void SetTutorialMonitor(bool isActive)
    {
        InTutorialMonitor = isActive;
    }

    public void SetInTutorialFalse()
    {
        InTutorial = false;
    }
    #endregion

    #region Game Pause Methods
    public void ToggleGamePause()
    {
        // This method is subscribed to the OnPause event from the InputHandler and 
        // is called whenever the player presses `esc`
        Scene scene = SceneManager.GetActiveScene();
        if (gameScenes.Contains(scene.name) && InTutorialMonitor == false)
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
            Debug.Log("GameManager: ToggleGamePause: Can't toggle. (This is correct if not in MainScene)");
        }
    }

    public void ActivateSubMenu()
    {
        GamePaused = true;
    }
    #endregion

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
                Debug.Log("GameManager: GetPlayerResourceValue: Getting resource value failed, not recognized resource type.");
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
        playerStatistics["Items Collected"] += amount;
        DebtUI.UpdateStatistics(playerStatistics);
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
                Debug.Log("Taking resource failed, not recognized resource type.");
                break;
        }
        PlayerUI.UpdateUI();
    }
    public void AddCreditsToPlayer(int amount)
    {
        PlayerCredits += amount;
        Debug.Log($"GameManager: AddCreditsToPlayer: Adding {amount} to PlayerCredits. Current value is {PlayerCredits}");
        PlayerUI.UpdateUI();
        playerStatistics["Credits Earned"] += amount;
        UpdateNetProfit();
        DebtUI.UpdateStatistics(playerStatistics);
    }

    public void TakeCreditsFromPlayer(int amount)
    {
        PlayerCredits -= amount;
        Debug.Log($"GameManager: TakeCreditsToPlayer: Taking {amount} from PlayerCredits. Current value is {PlayerCredits}");
        PlayerUI.UpdateUI();
        playerStatistics["Credits Spent"] += amount;
        UpdateNetProfit();
        DebtUI.UpdateStatistics(playerStatistics);
    }


    #endregion

    #region Cheatcodes

    private void Cheatcodes()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            AddResourceToPlayer(5, ResourceTypes.CROP);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            AddResourceToPlayer(5, ResourceTypes.PART);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddResourceToPlayer(5, ResourceTypes.NITROGEN);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddCreditsToPlayer(5000);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Time.timeScale = 5;
        }
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftControl))
        {
            Time.timeScale = 10;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            Time.timeScale = 1;
        }

    }

    #endregion
}
