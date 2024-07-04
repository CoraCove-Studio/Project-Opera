using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineSlot : MonoBehaviour
{
    private GameObject spawnedMachine;
    private InteractableObject interactableComponent;
    [SerializeField] List<GameObject> machinePrefabs;
    [SerializeField] Transform spawnLocation;
    [SerializeField] GameObject machineUICanvas;

    [SerializeField] Image floatingIconImage;
    [SerializeField] private Sprite bitcoinIcon;
    [SerializeField] private Sprite machineIcon;

    private Color white = Color.white;
    private Color red = Color.red;


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

    private IEnumerator ChangeToRedMoneyIcon()
    {
        floatingIconImage.sprite = bitcoinIcon;
        floatingIconImage.color = red;
        yield return new WaitForSeconds(1f);
        floatingIconImage.sprite = machineIcon;
        floatingIconImage.color = white;
    }

    public void SpawnMachine(ResourceTypes resourceType)
    {
        if (GameManager.Instance.PlayerCredits >= 50)
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
            GameManager.Instance.TakeCreditsFromPlayer(50);
            machineUICanvas.SetActive(false);
            interactableComponent.enabled = false;
            gameObject.layer = 0;
        }
        else
        {
            GameManager.Instance.audioManager.PlayErrorNoise();
            StartCoroutine(ChangeToRedMoneyIcon());
        }
    }
}
