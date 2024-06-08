using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    #region Singleton pattern and Update

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameManager = new("GameManager");
                instance = gameManager.AddComponent<GameManager>();
                DontDestroyOnLoad(gameManager);
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

    private void Update()
    {
        Cheatcodes();
        GameTimer();
    }

    #endregion

    [SerializeField] private string mainGameSceneName = "TestZeb";

    const int minResources = 0;
    const int maxResources = 300;

    [SerializeField] private PlayerUIHandler playerUI;

    public bool GamePaused { get; private set; } = true;
    private float timeCounter;

    private int playerCredits;

    private int playerCrops;
    private int playerParts;
    private int playerNitrogen;

    #region

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InputManager.Instance.OnPause += ToggleGamePause;

    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("Current scene is: " + scene.name);
        if (scene.name == mainGameSceneName)
        {
            playerUI = GameObject.Find("PlayerCanvas").GetComponent<PlayerUIHandler>();
            StartNewGame();
        }
    }

    #endregion

    public void StartNewGame()
    {
        // To be called when the main scene is loaded
        SetNewGameValues();
        playerUI.UpdateUI();
        Cursor.lockState = CursorLockMode.Locked;
        GamePaused = false;
    }

    public void GameTimer()
    {
        if (GamePaused == false)
        {
            timeCounter += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeCounter / 60f);
            int seconds = Mathf.FloorToInt(timeCounter - minutes * 60);
            if (playerUI != null)
            {
                playerUI.UpdateGameTimer(minutes, seconds);
            }
        }
    }

    private void RestartTimer()
    {
        timeCounter = 0;
    }

    private void ToggleGamePause()
    {
        if (GamePaused == false)
        {
            print("Pausing game.");
            Time.timeScale = 0;
            playerUI.EnablePauseMenu();
            // toggle action map?
            Cursor.lockState = CursorLockMode.Confined;
            GamePaused = true;
        }
        else
        {
            print("Resuming game.");
            Time.timeScale = 1;
            playerUI.DisablePauseMenu();
            // toggle action map
            Cursor.lockState = CursorLockMode.Locked;
            GamePaused = false;
        }
    }

    #region Resources Get/Set
    public int PlayerCrops
    {
        get { return playerCrops; }
        private set
        {
            playerCrops = value;
            playerCrops = Mathf.Clamp(playerCrops, minResources, maxResources);
            print($"Adding {value} to playerCrops. Current value is {playerCrops}");
            playerUI.UpdateUI();
        }
    }

    public int PlayerParts
    {
        get { return playerParts; }
        private set
        {
            playerParts = value;
            playerParts = Mathf.Clamp(playerParts, minResources, maxResources);
            playerUI.UpdateUI();
        }
    }

    public int PlayerNitrogen
    {
        get { return playerNitrogen; }
        private set
        {
            playerNitrogen = value;
            playerNitrogen = Mathf.Clamp(playerNitrogen, minResources, maxResources);
            playerUI.UpdateUI();
        }
    }

    public int PlayerCredits
    {
        get { return playerCredits; }
        private set
        {
            playerCredits = value;
            playerCredits = Mathf.Clamp(playerCredits, minResources, 10000);
            playerUI.UpdateUI();
        }
    }

    #endregion

    public void SetNewGameValues()
    {
        // Important that these are the lowercase local variables, not the setters
        // Because otherwise the gameManager will try to access UI elements that don't exist.
        playerCrops = 5;
        playerParts = 5;
        playerNitrogen = 5;
        playerCredits = 50;
    }

    #region Add / Take Resource Methods

    public void AddCropsToPlayer(int amount)
    {
        PlayerCrops += amount;
    }
    public void AddPartsToPlayer(int amount)
    {
        PlayerParts += amount;
    }
    public void AddNitrogenToPlayer(int amount)
    {
        PlayerNitrogen += amount;
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
                print("Adding resource failed, not recognized resource type.");
                break;
        }
    }

    public void AddCreditsToPlayer(int amount)
    {
        PlayerCredits += amount;
    }

    public void TakeCropsFromPlayer(int amount)
    {
        PlayerCrops -= amount;
    }
    public void TakePartsFromPlayer(int amount)
    {
        PlayerParts -= amount;
    }
    public void TakeNitrogenFromPlayer(int amount)
    {
        PlayerNitrogen -= amount;
    }

    public void TakeCreditsFromPlayer(int amount)
    {
        PlayerCredits -= amount;
    }

    public void TakeResourceToPlayer(int amount, ResourceTypes resourceType)
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
            RestartTimer();
        }
    }

    #endregion

    // Method which tracks the timer and has functionality to count down to a set time.
}
