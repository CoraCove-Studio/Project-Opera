using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MachineBehavior : MonoBehaviour
{
    [SerializeField] private int inputInventory;
    [SerializeField] private int machineEfficiency = 4;
    [SerializeField] private int outputInterval = 8;
    [SerializeField] private int maximumInventory = 20;
    [SerializeField] private ObjectPooler objPooler;
    [SerializeField] private Transform outputPos;
    [SerializeField] private MachineUI machineUI;

    private readonly Dictionary<ResourceTypes, ResourceTypes> resourceTypeRelationships = new();
    public abstract ResourceTypes MachineType { get; }
    private Coroutine productionCoroutine;

    protected virtual void OnEnable()
    {
        ConfigureRelationshipDictionary();
        objPooler = GameObject.Find("ObjectPooler").GetComponent<ObjectPooler>();
        machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
        productionCoroutine = StartCoroutine(Production());
    }

    protected virtual void OnDisable()
    {
        if (productionCoroutine != null)
        {
            StopCoroutine(productionCoroutine);
        }
    }

    private void ConfigureRelationshipDictionary()
    {
        resourceTypeRelationships.Add(ResourceTypes.CROP, ResourceTypes.NITROGEN);
        resourceTypeRelationships.Add(ResourceTypes.PART, ResourceTypes.CROP);
        resourceTypeRelationships.Add(ResourceTypes.NITROGEN, ResourceTypes.PART);
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
        if (GameManager.Instance.GetPlayerResourceValue(resourceTypeRelationships[MachineType]) > 0 && inputInventory < maximumInventory)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, resourceTypeRelationships[MachineType]);

            inputInventory += machineEfficiency;
            inputInventory = Mathf.Clamp(inputInventory, 0, maximumInventory);
            machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
        }
        else
        {
            Debug.Log("MachineBehavior: AddInput: Tried to add input, player didn't have enough.");
        }

    }

    public abstract void Upgrade(float reduction);
}
