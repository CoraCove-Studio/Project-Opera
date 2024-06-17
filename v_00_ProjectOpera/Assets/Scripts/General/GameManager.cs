using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Fields and Properties

    public string mainGameSceneName = "MainScene";
    public string testScene1 = "emma_MainScene";
    public string testScene2 = "rachel_MainScene";

    private const int minResources = 0;
    private const int maxResources = 300;

    public PlayerUIHandler PlayerUI { get; private set; }
    public GameObject Player { get; private set; }
    private Rigidbody playerRigidBody;
    private GameTimer gameTimer;
    private float gameDurationInSeconds;

    public bool GamePaused { get; private set; } = true;
    public bool IsInTutorial { get; private set; } = true;

    private int playerCredits;
    private int playerCrops;
    private int playerParts;
    private int playerNitrogen;

    private static bool isQuitting = false;

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
        if (gameTimer != null && PlayerUI != null && GamePaused == false)
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

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        print("Current scene is: " + scene.name);
        if (scene.name == mainGameSceneName || scene.name == "TestZeb" || scene.name == testScene1|| scene.name == testScene2)
        {
            SetUpNewGame();
        }
        else if (scene.name == "GameOver")
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    #endregion

    public void StartGame()
    {
        ToggleGamePause();
        gameTimer.StartTimer();
        playerRigidBody.isKinematic = false;
        IsInTutorial = false;
    }
    public void SetUpNewGame()
    {
        Debug.Log("GameManager: SetUpNewGame: Starting new game.");
        PlayerUI = GameObject.Find("PlayerCanvas").GetComponent<PlayerUIHandler>();
        GameObject.Find("GameTimer").TryGetComponent(out gameTimer);
        playerRigidBody = GameObject.Find("PlayerPrefab").GetComponent<Rigidbody>();

        SetNewGameValues();

        if (PlayerUI != null)
        {
            PlayerUI.UpdateUI();
            PlayerUI.ToggleReticleVisibility(false);
        }
        else
        {
            Debug.Log("PlayerUI not found by GameManager.");
        }

        if (gameTimer != null)
        {
            gameTimer.SetNewTimer(gameDurationInSeconds);
            PlayerUI.UpdateGameTimer(gameTimer.TimeLeft.Item1, gameTimer.TimeLeft.Item2);
        }
    }

    public void CheckGameOver()
    {
        if (gameTimer != null && gameTimer.CountdownTimer <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void ToggleGamePause()
    {
        // This method is subscribed to the OnPause event from the InputHandler and 
        // is called whenever the player presses `esc`
        UnityEngine.SceneManagement.Scene scene = SceneManager.GetActiveScene();
        if (scene.name == mainGameSceneName || scene.name == "TestZeb" || scene.name == testScene1 || scene.name == testScene2)
        {
            if (GamePaused == true)
            {
                Debug.Log("GameManager: ToggleGamePause: Resuming game.");
                //Time.timeScale = 1;
                if (PlayerUI != null)
                {
                    PlayerUI.ResumeGame();
                }
                GamePaused = false;
            }
            else
            {
                Debug.Log("GameManager: ToggleGamePause: Pausing game.");
                //Time.timeScale = 0;
                if (PlayerUI != null)
                {
                    PlayerUI.PauseGame();
                }
                GamePaused = true;
            }
        }
    }

    public void ActivateSubMenu()
    {
        GamePaused = true;
    }
    public void SetNewGameValues()
    {
        playerCrops = 5;
        playerParts = 5;
        playerNitrogen = 5;
        playerCredits = 300;
        gameDurationInSeconds = 300;
    }

    #region Resources Properties

    public int PlayerCrops
    {
        get { return playerCrops; }
        private set
        {
            playerCrops = Mathf.Clamp(value, minResources, maxResources);
            print($"Adding {value} to playerCrops. Current value is {playerCrops}");
            if (PlayerUI != null)
            {
                PlayerUI.UpdateUI();
            }
        }
    }

    public int PlayerParts
    {
        get { return playerParts; }
        private set
        {
            playerParts = Mathf.Clamp(value, minResources, maxResources);
            if (PlayerUI != null)
            {
                PlayerUI.UpdateUI();
            }
        }
    }

    public int PlayerNitrogen
    {
        get { return playerNitrogen; }
        private set
        {
            playerNitrogen = Mathf.Clamp(value, minResources, maxResources);
            if (PlayerUI != null)
            {
                PlayerUI.UpdateUI();
            }
        }
    }

    public int PlayerCredits
    {
        get { return playerCredits; }
        private set
        {
            playerCredits = Mathf.Clamp(value, minResources, 10000);
            if (PlayerUI != null)
            {
                PlayerUI.UpdateUI();
            }
        }
    }

    #endregion


    #region Add / Take Resource Methods

    public int GetPlayerResourceValue(ResourceTypes resourceType)
    {
        switch (resourceType)
        {
            case ResourceTypes.CROP:
                return PlayerCrops;
            case ResourceTypes.PART:
                return PlayerParts;
            case ResourceTypes.NITROGEN:
                return PlayerNitrogen;
            default:
                print("GameManager: GetPlayerResourceValue: Getting resource value failed, not recognized resource type.");
                return 0;
        }
    }

    public void AddResourceToPlayer(int amount, ResourceTypes resourceType)
    {
        switch (resourceType)
        {
            case ResourceTypes.CROP:
                PlayerCrops += amount;
                break;
            case ResourceTypes.PART:
                PlayerParts += amount;
                break;
            case ResourceTypes.NITROGEN:
                PlayerNitrogen += amount;
                break;
            default:
                print("GameManager: AddResourceToPlayer: Adding resource failed, not recognized resource type.");
                break;
        }
    }

    public void AddCreditsToPlayer(int amount)
    {
        PlayerCredits += amount;
    }

    public void TakeCreditsFromPlayer(int amount)
    {
        PlayerCredits -= amount;
    }

    public void TakeResourceFromPlayer(int amount, ResourceTypes resourceType)
    {
        switch (resourceType)
        {
            case ResourceTypes.CROP:
                PlayerCrops -= amount;
                break;
            case ResourceTypes.PART:
                PlayerParts -= amount;
                break;
            case ResourceTypes.NITROGEN:
                PlayerNitrogen -= amount;
                break;
            default:
                print("Taking resource failed, not recognized resource type.");
                break;
        }
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
