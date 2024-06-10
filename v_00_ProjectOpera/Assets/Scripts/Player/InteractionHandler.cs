using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private PlayerUIHandler playerUI;

    private InteractableObject currentInteractable;
    private Vector3 interactRayPoint = new(0.5f, 0.5f, 0); // Center of the screen

    private void Update()
    {
        InteractionCheck();
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteraction += InteractionInput;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteraction -= InteractionInput;
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

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance) && hit.collider.gameObject.layer == 3 )
        {
            // Check if the hit object's layer is in the interactableLayerMask
            if (((1 << hit.collider.gameObject.layer) & interactableLayerMask) != 0)
            {
                if (hit.collider.TryGetComponent(out currentInteractable))
                {
                    currentInteractable.OnFocus();
                    playerUI.EnableInteractReticle();
                }
            }
        }
        else
        {
            currentInteractable = null;
            playerUI.DisableInteractReticle();
        }
    }

    private void InteractionInput()
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnInteract();
        }
    }
}