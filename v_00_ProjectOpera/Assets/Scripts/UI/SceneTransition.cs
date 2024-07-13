using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] Animator animController;
    private EventSystem eventSystem;

    private void Awake()
    {
        GameObject.Find("EventSystem").TryGetComponent(out eventSystem);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            animController.SetBool("MainSceneOpen", true);
            animController.SetBool("SceneClose", false);
            animController.SetBool("StartButtonClicked", false);
        }
        else
        {
            animController.SetBool("MainSceneOpen", false);
            animController.SetBool("SceneClose", false);
            animController.SetBool("StartButtonClicked", false);
        }
    }

    public void StartSceneClose()
    {
        animController.SetBool("SceneClose", true);
    }

    public void OnStartButtonClick()
    {
        animController.SetBool("StartButtonClicked", true);
    }


    public void OnPointerEnter(GameObject self)
    {
        eventSystem.SetSelectedGameObject(self);
    }
    public void SelectButton(Image buttonImage)
    {
        buttonImage.color = new(255, 255, 255, 1);
    }

    public void DeselectButton(Image buttonImage)
    {
        buttonImage.color = new(255, 255, 255, 0);
    }
}
