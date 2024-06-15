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
        // print("LOOKING AT " + gameObject.name);
    }

    public override void OnInteract()
    {
        // print("INTERACTED WITH " + gameObject.name);
        Interact.Invoke();
    }

    public override void OnLoseFocus()
    {
        // print("STOPPED LOOKING AT " + gameObject.name);
    }
}
