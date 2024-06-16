using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class MachineBehavior : MonoBehaviour
{
    [SerializeField] private int inputInventory;
    [SerializeField] private int machineEfficiency = 4;
    [SerializeField] private int outputInterval = 2;
    [SerializeField] private ObjectPooler objPooler;
    [SerializeField] private Transform outputPos;
    private readonly Dictionary<ResourceTypes, ResourceTypes> resourceTypeRelationships = new();
    public abstract ResourceTypes MachineType { get; }
    private Coroutine productionCoroutine;

    protected virtual void OnEnable()
    {
        ConfigureRelationshipDictionary();
        objPooler = GameObject.Find("ObjectPooler").GetComponent<ObjectPooler>();
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
                product = objPooler.ReturnProduct(MachineType);
                ConfigureProduct(product);
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
        GameManager.Instance.TakeResourceFromPlayer(1, resourceTypeRelationships[MachineType]);
        inputInventory += machineEfficiency;
    }

    public abstract void Upgrade(float reduction);
}
