using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] private Image playerReticle;
    [SerializeField] private List<Sprite> reticles = new();
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private List<TextMeshProUGUI> resourceLabels = new();
    [SerializeField] private TextMeshProUGUI timerLabel;

    private readonly string[] labelText = {
        "CROPS: ",
        "PARTS: ",
        "NITROGEN: ",
        "CREDITS: "
    };


    private bool firstButtonSelected = false;

    private void Awake()
    {
        UpdateUI();

    }

    public void EnablePauseMenu()
    {
        pauseMenu.SetActive(true);
        ToggleReticleVisibility();
    }

    public void DisablePauseMenu()
    {
        pauseMenu.SetActive(false);
        ToggleReticleVisibility();
    }

    #region Button Methods

    public void OnClickResumeButton()
    {
        DisablePauseMenu();
    }

    public void OnClickMainMenuButton()
    {

    }

    public void OnClickQuitButton()
    {

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

    public void ToggleReticleVisibility()
    {
        playerReticle.gameObject.SetActive(!playerReticle.gameObject.activeSelf);
    }

    public void UpdateUI()
    {
        List<int> playerResources = GetPlayerResources();

        for (int i = 0; i < resourceLabels.Count; i++)
        {
            resourceLabels[i].text = labelText[i] + playerResources[i].ToString();
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
}
