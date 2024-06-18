using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MachineBehavior : MonoBehaviour
{
    [SerializeField] protected int inputInventory;
    [SerializeField] protected int machineEfficiency = 4;
    [SerializeField] protected int outputInterval = 8;
    [SerializeField] protected int maximumInventory = 20;
    [SerializeField] protected int outputIntervalLevel = 1;
    [SerializeField] protected int machineEfficiencyLevel = 1;

    [SerializeField] private ObjectPooler objPooler;
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

    protected virtual void OnEnable()
    {
        objPooler = GameObject.Find("ObjectPooler").GetComponent<ObjectPooler>();
        machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
        machineUI.SetSliderMaxValue(outputInterval);
        productionCoroutine = StartCoroutine(Production());
    }

    protected virtual void OnDisable()
    {
        if (productionCoroutine != null)
        {
            StopCoroutine(productionCoroutine);
        }
    }

    private IEnumerator Production()
    {
        GameObject product;

        while (true)
        {
            if (inputInventory > 0)
            {
                inputInventory--;
                machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
                product = objPooler.ReturnProduct(MachineType);
                ConfigureProduct(product);
                machineUI.StartBarAnimation(outputInterval);
            }

            yield return new WaitForSeconds(outputInterval);
        }
    }

    private void ConfigureProduct(GameObject product)
    {
        product.transform.position = outputPos.position;
        product.SetActive(true);
    }

    public void AddInput()
    {
        if (GameManager.Instance.CheckPlayerResourceValue(1, resourceTypeRelationships[MachineType]) && inputInventory < maximumInventory)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, resourceTypeRelationships[MachineType]);

            inputInventory += machineEfficiency;
            inputInventory = Mathf.Clamp(inputInventory, 0, maximumInventory);
            machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
        }
        else
        {
            Debug.Log($"MachineBehavior: AddInput: Couldn't add {resourceTypeRelationships[MachineType]}. Machine full or player didn't have enough.");
        }

    }

    //upgrades how many products are produced with one input
    public abstract void UpgradeMachineEfficiency(int increase);

    //upgrades how quickly the machines produce one product
    public abstract void UpgradeOutputInterval(int reduction);
}
