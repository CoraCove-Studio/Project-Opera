using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineSlot : MonoBehaviour
{
    private GameObject spawnedMachine;
    private InteractableObject interactableComponent;
    [SerializeField] List<GameObject> machinePrefabs;
    [SerializeField] Transform spawnLocation;

    private PlayerUIHandler playerUI;

    private void Start()
    {
        playerUI = GameManager.Instance.PlayerUI;
        interactableComponent = GetComponent<InteractableObject>();
    }

    public void OnInteract()
    {
        playerUI.ActivateMachineSpawnPanel(this);
    }

    public void SpawnMachine(ResourceTypes resourceType)
    {
        switch (resourceType)
{
            case ResourceTypes.CROP:
                Instantiate(machinePrefabs[0], spawnLocation.position, spawnLocation.rotation);
                break;
            case ResourceTypes.PART:
                Instantiate(machinePrefabs[1], spawnLocation.position, spawnLocation.rotation);
                break;
            case ResourceTypes.NITROGEN:
                Instantiate(machinePrefabs[2], spawnLocation.position, spawnLocation.rotation);
                break;
            default:
                break;
        }

        interactableComponent.enabled = false;
        gameObject.layer = 0;
    }
}
