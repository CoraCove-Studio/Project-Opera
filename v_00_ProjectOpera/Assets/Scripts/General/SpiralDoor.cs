using UnityEngine;

public class SpiralDoor : MonoBehaviour
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
            animator.SetBool("open", true);
            audioSource.PlayOneShot(doorOpen);
            isOpen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isOpen)
        {
            animator.SetBool("open", false);
            audioSource.PlayOneShot(doorClose);
            isOpen = false;
        }
    }
}
