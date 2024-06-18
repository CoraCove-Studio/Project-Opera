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
    [SerializeField] private GameObject machineSpawnPanel;
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private List<TextMeshProUGUI> resourceLabels = new();
    [SerializeField] private TextMeshProUGUI timerLabel;

    public GameObject currentPanel;
    private MachineSlot currentMachineSlot;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private bool firstButtonSelected = false;

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
        SceneManager.LoadScene("MainMenu");
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
    }

    public void UpdateUI()
    {
        List<int> playerResources = GetPlayerResources();

        for (int i = 0; i < resourceLabels.Count; i++)
        {
            resourceLabels[i].text = playerResources[i].ToString();
        }
    }

    public void UpdateGameTimer(int minutes, int seconds)
    {
        timerLabel.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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
        // on whether or not the player has enough credits

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
}
