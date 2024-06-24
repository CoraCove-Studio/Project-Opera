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
    [SerializeField] private GameObject creditsPanel;

    private bool firstButtonSelected = false;

    // Start is called before the first frame update
    void Start()
    {
 
    }

    #region Button Methods

    public void OnClickStartButton()
    {
        SceneManager.LoadScene(GameManager.Instance.mainGameSceneName);
    }

    public void OnClickHelpButton()
    {
        SwapActivePanel(helpPanel);
    }
    public void OnClickSettingsButton()
    {
        SwapActivePanel(settingsPanel);
    }
    public void OnClickCreditsButton()
    {
        SwapActivePanel(creditsPanel);
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