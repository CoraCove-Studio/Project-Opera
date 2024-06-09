using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Fields and Properties

    public string mainGameSceneName = "TestZeb";

    private const int minResources = 0;
    private const int maxResources = 300;

    public PlayerUIHandler PlayerUI { get; private set; }
    private GameTimer gameTimer;
    private float gameDurationInSeconds;

    public bool GamePaused { get; private set; } = false;

    private int playerCredits;
    private int playerCrops;
    private int playerParts;
    private int playerNitrogen;

    #endregion

    #region Singleton pattern

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

    #endregion

    private void Update()
    {
        Cheatcodes();
        if (gameTimer != null && PlayerUI != null)
        {
            PlayerUI.UpdateGameTimer(gameTimer.TimeLeft.Item1, gameTimer.TimeLeft.Item2);
            CheckGameOver();
        }
    }


    public void StartNewGame()
    {
        Debug.Log("GameManager: StartNewGame: Starting new game.");
        PlayerUI = GameObject.Find("PlayerCanvas").GetComponent<PlayerUIHandler>();
        gameTimer = GameObject.Find("GameTimer").GetComponent<GameTimer>();
        SetNewGameValues();
        if (PlayerUI != null)
        {
            PlayerUI.UpdateUI();
        }
        else
        {
            Debug.Log("PlayerUI not found by GameManager.");
        }
        Cursor.lockState = CursorLockMode.Locked;
        if (GamePaused == true)
        {
            ToggleGamePause();
        }
        if (gameTimer != null)
        {
            gameTimer.SetNewTimer(gameDurationInSeconds);
            gameTimer.StartTimer();
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
        if (SceneManager.GetActiveScene().name == mainGameSceneName)
        {
            if (GamePaused == true)
            {
                Debug.Log("GameManager: ToggleGamePause: Resuming game.");
                //Time.timeScale = 1;
                if (PlayerUI != null)
                {
                    PlayerUI.DisablePauseMenu();
                }
                Cursor.lockState = CursorLockMode.Locked;
                GamePaused = false;
            }
            else
            {
                Debug.Log("GameManager: ToggleGamePause: Pausing game.");
                //Time.timeScale = 0;
                if (PlayerUI != null)
                {
                    PlayerUI.EnablePauseMenu();
                }
                Cursor.lockState = CursorLockMode.Confined;
                GamePaused = true;
            }
        }
    }
    public void SetNewGameValues()
    {
        playerCrops = 5;
        playerParts = 5;
        playerNitrogen = 5;
        playerCredits = 0;
        gameDurationInSeconds = 300;
    }

    #region Unity Event Methods

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InputManager.Instance.OnPause += ToggleGamePause;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        InputManager.Instance.OnPause -= ToggleGamePause;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("Current scene is: " + scene.name);
        if (scene.name == mainGameSceneName)
        {
            StartNewGame();
        }
        else if (scene.name == "GameOver")
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    #endregion

    #region Resources Get/Set

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
            // restart timer
        }
    }

    #endregion
}
