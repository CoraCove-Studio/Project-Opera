using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class MachineBehavior : MonoBehaviour
{
    [SerializeField] protected int inputInventory;
    [SerializeField] protected int machineEfficiency = 4;
    [SerializeField] protected int outputInterval = 8;
    [SerializeField] protected int outputIntervalLevel = 1;
    [SerializeField] protected int machineEfficiencyLevel = 1;
    [SerializeField] protected int maximumInventory = 20;
    [SerializeField] protected int machineDurability = 100;
    [SerializeField] protected int maximumMachineDurability = 100;
    [SerializeField] protected int upgradeCost = 35;

    [SerializeField] protected ObjectPooler objPooler;
    [SerializeField] private Transform outputPos;
    [SerializeField] protected MachineUI machineUI;


    private readonly Dictionary<ResourceTypes, ResourceTypes> resourceTypeRelationships = new()
    {
        { ResourceTypes.CROP, ResourceTypes.NITROGEN },
        { ResourceTypes.PART, ResourceTypes.CROP },
        { ResourceTypes.NITROGEN, ResourceTypes.PART }
    };

    public abstract ResourceTypes MachineType { get; }
    private Coroutine productionCoroutine;

    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected List<AudioClip> machineProductionLoops;
    [SerializeField] private AudioClip productInstantiationNoise;

    protected virtual void OnEnable()
    {
        objPooler = GameObject.Find("ObjectPooler").GetComponent<ObjectPooler>();
        machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
        machineUI.SetSliderMaxValue(outputInterval);
        audioSource = GetComponent<AudioSource>();
        GetProductionAudioClip();
        audioSource.loop = true;
        productionCoroutine = StartCoroutine(Production());
    }

    protected virtual void OnDisable()
    {
        if (productionCoroutine != null)
        {
            StopCoroutine(productionCoroutine);
        }
    }
    protected virtual IEnumerator Production()
    {
        GameObject product;

        while (true)
        {
            if (inputInventory > 0 & machineDurability > 0)
            {
                audioSource.Play();
                // Don't reorder StartBarAnimation, inventory decrementation and WaitForSeconds()
                machineUI.StartBarAnimation(outputInterval);
                inputInventory--;
                machineDurability -= 10;
                yield return new WaitForSeconds(outputInterval);
                for (int i = 0; i < machineEfficiency; i++)
                {
                    machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
                    product = objPooler.ReturnProduct(MachineType);
                    ConfigureProduct(product);
                }
                machineUI.UpdateDurabilityBar(machineDurability);
            }
            else
            {
                audioSource.Stop();
                yield return new WaitForSeconds(0.2f);
            }
        }
    }


    protected void ConfigureProduct(GameObject product)
    {
        product.transform.position = outputPos.position;
        product.SetActive(true);
        audioSource.PlayOneShot(productInstantiationNoise);
    }

    public void AddInput()
    {
        if (GameManager.Instance.CheckPlayerResourceValue(1, resourceTypeRelationships[MachineType]) && inputInventory < maximumInventory && machineDurability > 0)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, resourceTypeRelationships[MachineType]);
            if (inputInventory == 0) machineUI.StartBarAnimation(outputInterval); // Touch this with care
            inputInventory += 1;
            inputInventory = Mathf.Clamp(inputInventory, 0, maximumInventory);
            machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
        }
        else
        {
            Debug.Log($"MachineBehavior: AddInput: Couldn't add {resourceTypeRelationships[MachineType]}. Machine full or player didn't have enough.");
        }

    }

    public void DisplayInput()
    {
        GameManager.Instance.DisplayTooltip(resourceTypeRelationships[MachineType], -1);
    }

    public void DisplayPrice()
    {
        GameManager.Instance.DisplayTooltip(-upgradeCost);
    }

    private void GetProductionAudioClip()
    {
        audioSource.clip = machineProductionLoops[Random.Range(0, machineProductionLoops.Count)];
    }

    //repairs the machines for credits
    public abstract void RepairMachine();

    //upgrades how many products are produced with one input
    public abstract void UpgradeMachineEfficiency(int change);
}
