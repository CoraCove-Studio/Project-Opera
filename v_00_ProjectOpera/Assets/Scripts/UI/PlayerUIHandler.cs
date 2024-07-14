using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] private Image playerReticle;
    [SerializeField] private List<Sprite> reticles = new();
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private GameObject machineSpawnPanel;
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private List<TextMeshProUGUI> resourceLabels = new();
    [SerializeField] private List<Button> machineSelectionButtons;
    [SerializeField] private TextMeshProUGUI timerLabel;

    private float fadeTime = 0.2f;
    private float smallTextSize = 55f;
    private float largeTextSize = 75f;
    private Dictionary<TextMeshProUGUI, Coroutine> activeCoroutines = new();

    public ToolTipHandler tooltipHandler;

    public GameObject tutorialLaunchPanel;
    public GameObject currentPanel;
    private MachineSlot currentMachineSlot;

    [SerializeField] private TradeTransitTicker transitTicker;
    [SerializeField] private NotificationTab notificationTab;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private bool firstButtonSelected = false;

    public void ActivateTicker()
    {
        transitTicker.ActivateTicker();
    }

    public void SendTimedNotification(string message)
    {
        notificationTab.ShowTimedNotification(message);
    }

    public void SendConditionalNotification(string message)
    {
        notificationTab.ShowConditionalNotification(message);
    }

    public void CloseConditionalNotification()
    {
        notificationTab.MeetCondition();
    }

    public List<Button> ReturnListOfMachineSelectionButtons()
    {
        return machineSelectionButtons;
    }

    public void PauseGame()
    {
        Debug.Log("PlayerUIHandler: PauseGame: Pausing the game.");
        pauseMenu.SetActive(true);
        currentPanel = pauseMenu;
        ToggleReticleVisibility(false);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame()
    {
        Debug.Log("PlayerUIHandler: PauseGame: Resuming the game.");
        if (currentPanel != null)
        {
            if (currentPanel == pauseMenu)
            {
                pauseMenu.SetActive(false);
            }
            else if (currentPanel == machineSpawnPanel)
            {
                machineSpawnPanel.SetActive(false);
            }
            currentPanel = null;
        }
        ToggleReticleVisibility(true);
        Cursor.lockState = CursorLockMode.Locked;
    }

    #region Button Methods

    public void OnClickResumeButton()
    {
        GameManager.Instance.ToggleGamePause();
        InputManager.Instance.UnpauseWithButton();
    }

    public void OnClickMainMenuButton()
    {
        Time.timeScale = 1;
        GameManager.Instance.SceneTransition.StartSceneClose();
        Invoke(nameof(LoadMainMenu), 1f);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnClickSettingsButton()
    {
        buttonPanel.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void OnClickBackButton()
    {
        settingsMenu.SetActive(false);
        buttonPanel.SetActive(true);
    }

    public void OnClickQuitButton()
    {
        Application.Quit();
    }

    #endregion

    #region EventSystem Methods

    public void OnPointerEnter(GameObject self)
    {
        eventSystem.SetSelectedGameObject(self);
    }
    public void SelectButton(Image buttonImage)
    {
        buttonImage.color = new(255, 255, 255, 1);

        // the following code prevents a sound from being played on start
        if (firstButtonSelected == false)
        {
            firstButtonSelected = true;
            return;
        }
        else
        {
            // play button noise
        }
    }

    public void DeselectButton(Image buttonImage)
    {
        buttonImage.color = new(255, 255, 255, 0);
    }

    #endregion

    public void EnableInteractReticle()
    {
        playerReticle.sprite = reticles[1];
    }

    public void DisableInteractReticle()
    {
        playerReticle.sprite = reticles[0];
    }

    public void ToggleReticleVisibility(bool trueOrFalse)
    {
        playerReticle.gameObject.SetActive(trueOrFalse);
        if (trueOrFalse)
        {
            tooltipHandler.EnableTooltipPanel();
        }
        else
        {
            tooltipHandler.DisableTooltipPanel();
        }
    }

    public void DisplayTooltip(int value)
    {
        tooltipHandler.DisplayValue(value);
    }

    public void DisplayTooltip(ResourceTypes resourceType, int value)
    {
        tooltipHandler.DisplayResource(resourceType, value);
    }

    public void UpdateUI()
    {
        List<int> playerResources = GetPlayerResources();

        if (playerResources.Count != resourceLabels.Count)
        {
            Debug.LogError("Resource labels count does not match player resources count.");
            return;
        }

        for (int i = 0; i < resourceLabels.Count; i++)
        {
            var currentLabel = resourceLabels[i];
            if (int.TryParse(currentLabel.text, out int labelToInt))
            {
                if (GameManager.Instance.GamePaused == false)
                {
                    if (labelToInt < playerResources[i])
                    {
                        StartDisplayLabelChange(currentLabel, Color.green);
                    }
                    else if (labelToInt > playerResources[i])
                    {
                        StartDisplayLabelChange(currentLabel, Color.red);
                    }
                }
                currentLabel.text = playerResources[i].ToString();
            }
            else
            {
                Debug.LogWarning($"Failed to parse text in label {i} to int.");
            }
        }
    }

    private void StartDisplayLabelChange(TextMeshProUGUI textLabel, Color color)
    {
        if (activeCoroutines.ContainsKey(textLabel))
        {
            StopCoroutine(activeCoroutines[textLabel]);
            activeCoroutines.Remove(textLabel);
        }
        Coroutine coroutine = StartCoroutine(DisplayColorChange(textLabel, color));
        activeCoroutines[textLabel] = coroutine;
    }

    private IEnumerator DisplayColorChange(TextMeshProUGUI textLabel, Color color)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            textLabel.color = Color.Lerp(color, Color.white, elapsedTime / fadeTime);
            textLabel.fontSize = Mathf.Lerp(smallTextSize, largeTextSize, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        textLabel.color = Color.white;
        textLabel.fontSize = smallTextSize;
        activeCoroutines.Remove(textLabel);
    }

    public void UpdateGameTimer(int minutes, int seconds)
    {
        timerLabel.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ActivateTradeTimer()
    {
        // timerLabel.fontSize 
    }

    private List<int> GetPlayerResources()
    {
        List<int> resourceList = new()
        {
            GameManager.Instance.PlayerCrops,
            GameManager.Instance.PlayerParts,
            GameManager.Instance.PlayerNitrogen,
            GameManager.Instance.PlayerCredits,
        };

        return resourceList;
    }

    #region Machine Spawn Panel Methods

    public void ActivateMachineSpawnPanel(MachineSlot machineSlot)
    {
        GameManager.Instance.ActivateSubMenu();
        currentMachineSlot = machineSlot;
        // loop through the buttons of the spawn panel and make them interactable or not interactable based
        // on whether or not the player is in the right tutorial stage

        machineSpawnPanel.SetActive(true);
        currentPanel = machineSpawnPanel;
        InputManager.Instance.PauseWithButton();
        ToggleReticleVisibility(false);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void DeactivateMachineSpawnPanel()
    {
        currentMachineSlot = null;
        OnClickResumeButton();
    }

    public void OnSpawnButtonClick(string resourceType)
    {
        switch (resourceType.ToUpper())
        {
            case "CROP":
                currentMachineSlot.SpawnMachine(ResourceTypes.CROP);
                break;
            case "PART":
                currentMachineSlot.SpawnMachine(ResourceTypes.PART);
                break;
            case "NITROGEN":
                currentMachineSlot.SpawnMachine(ResourceTypes.NITROGEN);
                break;
            default:
                break;
        }
        DeactivateMachineSpawnPanel();
    }
    #endregion

    #region Launch Game From Tutorial
    public void ActivateTutorialLaunchPanel()
    {
        GameManager.Instance.ActivateSubMenu();
        GameManager.Instance.SetTutorialMonitor(true);
        tutorialLaunchPanel.SetActive(true);
        currentPanel = tutorialLaunchPanel;
        InputManager.Instance.PauseWithButton();
        ToggleReticleVisibility(false);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OnClickStartGameFromTutorial()
    {
        GameManager.Instance.StartGameFromTutorial();
        currentPanel.SetActive(false);
        currentPanel = null;
        GameManager.Instance.ToggleGamePause();
        InputManager.Instance.UnpauseWithButton();
    }

    #endregion
}
