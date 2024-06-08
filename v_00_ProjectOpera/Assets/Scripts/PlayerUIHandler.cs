using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] private Image playerReticle;
    [SerializeField] private List<Sprite> reticles = new();
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private EventSystem eventSystem;

    private bool firstButtonSelected = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnablePauseMenu()
    {
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void DisablePauseMenu()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
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
        
    }
}
