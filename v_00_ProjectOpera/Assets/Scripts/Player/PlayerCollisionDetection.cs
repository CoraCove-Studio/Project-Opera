using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetection : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagManager.DOOR))
        {
            other.gameObject.GetComponent<DoorAnim>().SetBoolTrue();
            Debug.Log("PlayerCollisionDetection: OnTriggerEnter: Door Anim Set to True");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagManager.DOOR))
        {
            other.gameObject.GetComponent<DoorAnim>().SetBoolFalse();
            Debug.Log("PlayerCollisionDetection: OnTriggerEnter: Door Anim Set to False");
        }
    }
}
