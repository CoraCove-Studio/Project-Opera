using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip doorOpen;
    [SerializeField] AudioClip doorClose;

    private bool isOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            animator.SetBool("Open", true);
            audioSource.PlayOneShot(doorOpen);
            isOpen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isOpen)
        {
            animator.SetBool("Open", false);
            audioSource.PlayOneShot(doorClose);
            isOpen = false;
        }
    }
}
