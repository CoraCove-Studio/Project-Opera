using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private LayerMask interactLayer;
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

        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject.layer == 3 )
        {
            if (hit.collider.TryGetComponent(out currentInteractable))
            {
                currentInteractable.OnFocus();
            }
        }
        else
        {
            currentInteractable = null;
        }
    }

    private void InteractionInput()
    {
        print("Interaction attempted.");
        if (currentInteractable != null)
        {
            print("Interacted with " + currentInteractable.name);
            currentInteractable.OnInteract();
        }
    }
}