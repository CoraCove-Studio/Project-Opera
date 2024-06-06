using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] Image playerReticle;
    [SerializeField] List<Sprite> reticles = new();

    public void EnableInteractReticle()
    {
        playerReticle.sprite = reticles[1];
    }

    public void DisableInteractReticle()
    {
        playerReticle.sprite = reticles[0];
    }

    
}
