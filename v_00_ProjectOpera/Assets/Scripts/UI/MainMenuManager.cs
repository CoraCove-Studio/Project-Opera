using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject activePanel;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject scoreboardPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private List<AudioClip> buttonSelectClips;
    private AudioSource audioSource;
    private bool firstButtonSelected = false;

    [SerializeField] private SceneTransition sceneTransition;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private AudioClip GetRandomNoiseClip()
    {
        return buttonSelectClips[Random.Range(0, buttonSelectClips.Count)];
    }

    #region Button Methods

    public void OnClickStartButton()
    {
        sceneTransition.StartSceneClose();
        Invoke(nameof(LoadMainScene), 1f);
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene(GameManager.Instance.mainGameSceneName);
    }

    public void PlayClickNoise()
    {
        audioSource.PlayOneShot(GetRandomNoiseClip());
    }

    public void OnClickHelpButton()
    {
        SwapActivePanel(helpPanel);
        PlayClickNoise();
    }
    public void OnClickSettingsButton()
    {
        SwapActivePanel(settingsPanel);
        PlayClickNoise();
    }
    public void OnClickScoreboardButton()
    {
        SwapActivePanel(scoreboardPanel);
        PlayClickNoise();
    }
    public void OnClickCreditsButton()
    {
        SwapActivePanel(creditsPanel);
        PlayClickNoise();
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
    private void SwapActivePanel(GameObject panel)
    {
        if (activePanel != null)
        {
            activePanel.SetActive(false);
        }
        panel.SetActive(true);
        activePanel = panel;
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
}