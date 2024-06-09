using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetection : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagManager.CROP) || other.CompareTag(TagManager.PART) || other.CompareTag(TagManager.NITROGEN))
        {
            print(other.gameObject + " detected");
        }
    }
}
