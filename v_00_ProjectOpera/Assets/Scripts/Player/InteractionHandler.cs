using System.Collections;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private PlayerUIHandler playerUI;

    private InteractableObject currentInteractable;
    private Vector3 interactRayPoint = new(0.5f, 0.5f, 0); // Center of the screen

    private bool isHoldingInteraction;
    private Coroutine interactionCoroutine;

    private float minHoldTime = 0.5f; // Minimum hold time before it starts accelerating
    private float maxFrequency = 0.05f; // Maximum frequency of interactions
    private float accelerationDuration = 1f; // Time to reach max frequency

    private void Update()
    {
        InteractionCheck();
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractionStart += InteractionStart;
            InputManager.Instance.OnInteractionStop += InteractionStop;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractionStart -= InteractionStart;
            InputManager.Instance.OnInteractionStop -= InteractionStop;
        }
    }

    private void InteractionCheck()
    {
        if (playerCam == null)
        {
            print("Camera not found!"); // Ensure playerCam is not null
            return;
        }

        Ray ray = playerCam.ViewportPointToRay(interactRayPoint);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayerMask, QueryTriggerInteraction.Ignore)) //&& hit.collider.gameObject.layer == 3)
        {
            // No need to check the layer again here, it's already filtered by interactableLayerMask
            if (hit.collider.TryGetComponent(out currentInteractable))
            {
                currentInteractable.OnFocus();
                playerUI.EnableInteractReticle(currentInteractable.gameObject);
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable.OnLoseFocus();
                currentInteractable = null;
                playerUI.DisableInteractReticle();
            }
        }
    }

    private void InteractionStart()
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnInteract();
            isHoldingInteraction = true;
            interactionCoroutine = StartCoroutine(HoldInteractionCoroutine());
        }
    }

    private void InteractionStop()
    {
        if (isHoldingInteraction)
        {
            isHoldingInteraction = false;
            if (interactionCoroutine != null)
            {
                StopCoroutine(interactionCoroutine);
            }
        }
    }

    private IEnumerator HoldInteractionCoroutine()
    {
        float startTime = Time.time; // Record the start time
        float delay = minHoldTime;

        while (isHoldingInteraction)
        {
            yield return new WaitForSeconds(delay);
            if (!isHoldingInteraction) yield break;

            if (currentInteractable != null)
            {
                currentInteractable.OnHold();
            }

            // Calculate how much time has passed since the coroutine started
            float elapsedTime = Time.time - startTime;
            // Update the delay to gradually reduce it to the maximum frequency
            delay = Mathf.Lerp(minHoldTime, maxFrequency, Mathf.Clamp01(elapsedTime / accelerationDuration));
        }
    }

}
