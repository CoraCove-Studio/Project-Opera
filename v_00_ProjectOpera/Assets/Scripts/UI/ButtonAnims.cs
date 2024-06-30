using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnims : MonoBehaviour
{
    [SerializeField] private Animator button_Animator;

    private bool clickedToTurnOffPanel = false;
    private bool triggerChecked = false;

    // Start is called before the first frame update
    void Start()
    {
        button_Animator = GetComponent<Animator>();
    }

    public void SetAnimBool()
    {
        if (!clickedToTurnOffPanel && triggerChecked == false)
        {
            button_Animator.SetBool("isPanelOff", true);
            triggerChecked = true;
        }
        else
        {
            button_Animator.SetBool("isPanelOff", clickedToTurnOffPanel);
            triggerChecked = false;
        }
    }

}
