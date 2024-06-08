using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

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

        SetNewGameValues();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            AddCropsToPlayer(1);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            AddPartsToPlayer(1);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddNitrogenToPlayer(1);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddCreditsToPlayer(1);
        }
    }

    #endregion

    const int minResources = 0;
    const int maxResources = 300;

    [SerializeField] private PlayerUIHandler playerUI;

    private int playerCredits;

    private int playerCrops;
    private int playerParts;
    private int playerNitrogen;

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

    private void SetNewGameValues()
    {
        PlayerCrops = 5;
        PlayerParts = 5;
        PlayerNitrogen = 5;
        PlayerCredits = 50;
    }

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

    public void AddCreditsToPlayer(int amount)
    {
        PlayerCredits += amount;
    }

    // methods to pause and unpause the game by stopping time. Should subscribe to the OnPause and OnResume events.

    // Method which tracks the timer and has functionality to count down to a set time.

    // Methods to restart the timer to default value, or enable/disable it when things like going to the main menu happen.


}
