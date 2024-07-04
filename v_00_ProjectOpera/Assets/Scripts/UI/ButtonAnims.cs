using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnims : MonoBehaviour
{
    [SerializeField] private Animator buttonAnimator;

    private bool isPanelOff = false;
    private bool isTriggerChecked = false;

    // Start is called before the first frame update
    void Start()
    {
        if (buttonAnimator == null)
        {
            buttonAnimator = GetComponent<Animator>();
        }
    }

    public void TogglePanelAnimation()
    {
        if (buttonAnimator == null)
        {
            Debug.LogWarning("Animator component is missing. Cannot toggle animation.");
            return;
        }

        if (!isPanelOff && !isTriggerChecked)
        {
            buttonAnimator.SetBool("isPanelOff", true);
            isTriggerChecked = true;
        }
        else
        {
            isPanelOff = !isPanelOff;
            buttonAnimator.SetBool("isPanelOff", isPanelOff);
            isTriggerChecked = false;
        }
    }
}
