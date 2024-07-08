using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : Interactable
{
    public UnityEvent Focus;
    public UnityEvent Interact;
    public UnityEvent LoseFocus;

    public override void OnFocus()
    {
        Focus.Invoke();
    }

    public override void OnInteract()
    {
        Interact.Invoke();
    }

    public override void OnLoseFocus()
    {
        LoseFocus.Invoke();
        GameManager.Instance.ClearTooltipDisplay();
    }
}
