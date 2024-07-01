using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnim : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetBoolTrue()
    {
        anim.SetBool("IsClicked", true);
    }

    public void SetBoolFalse()
    {
        anim.SetBool("IsClicked", false);
    }

}
