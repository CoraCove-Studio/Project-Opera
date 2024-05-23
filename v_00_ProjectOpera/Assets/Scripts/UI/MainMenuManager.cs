using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;

    private bool firstButtonSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(GameObject self)
    {
        eventSystem.SetSelectedGameObject(self);
    }
    public void PlayButtonSelectClip()
    {
        if (firstButtonSelected == false)
        {
            firstButtonSelected = true;
            return;
        }
        print("New button selected.");
    }
}
