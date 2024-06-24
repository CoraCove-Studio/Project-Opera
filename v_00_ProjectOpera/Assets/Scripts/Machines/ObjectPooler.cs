using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [Header("Active Objects")]
    [SerializeField] List<GameObject> listOfCrops = new();
    [SerializeField] List<GameObject> listOfParts = new();
    [SerializeField] List<GameObject> listOfNitrogen = new();

    [Header("Pooled Prefabs")]
    [SerializeField] List<GameObject> listOfCropPrefabs = new();
    [SerializeField] List<GameObject> listOfPartPrefabs = new();
    [SerializeField] List<GameObject> listOfNitrogenPrefabs = new();

    #region general methods

    public GameObject ReturnProduct(ResourceTypes resourceType)
    {
        GameObject productToReturn;
        productToReturn = ReturnInactiveObject(resourceType);

        if (productToReturn == null)
        {
            productToReturn = GetNewObject(resourceType);
        }
        return productToReturn;
    }
    private GameObject ReturnInactiveObject(ResourceTypes resourceType)
    {
        switch (resourceType)
        {
            case ResourceTypes.CROP:
                foreach (GameObject gameObject in listOfCrops)
                {
                    if (gameObject.activeInHierarchy == false)
                    {
                        return gameObject;
                    }
                }
                break;

            case ResourceTypes.PART:
                foreach (GameObject gameObject in listOfParts)
                {
                    if (gameObject.activeInHierarchy == false)
                    {
                        return gameObject;
                    }
                }
                break;

            case ResourceTypes.NITROGEN:
                foreach (GameObject gameObject in listOfNitrogen)
                {
                    if (gameObject.activeInHierarchy == false)
                    {
                        return gameObject;
                    }
                }
                break;
            default:
                Debug.Log("ObjectPooler: ReturnInactiveObject: Error, default switch case.");
                return _ = Instantiate(listOfCropPrefabs[0]);
        }

        return null;
    }

private GameObject GetNewObject(ResourceTypes resourceType)
    {
        GameObject newObject;

        switch (resourceType)
        {
            case ResourceTypes.CROP:
                newObject = Instantiate(listOfCropPrefabs[0]);
                ConfigureNewObject(newObject, resourceType);
                return newObject;

            case ResourceTypes.PART:
                newObject = Instantiate(listOfPartPrefabs[0]);
                ConfigureNewObject(newObject, resourceType);
                return newObject;

            case ResourceTypes.NITROGEN:
                newObject = Instantiate(listOfNitrogenPrefabs[0]);
                ConfigureNewObject(newObject, resourceType);
                return newObject;
            default:
                Debug.Log("ObjectPooler: GetNewObject: Error, default switch case.");
                return _ = Instantiate(listOfCropPrefabs[0]);
        }
    }

    private void ConfigureNewObject(GameObject newObject, ResourceTypes resourceType)
    {
        Product _ = newObject.GetComponent<Product>();
        _.SetObjectPoolerReference(this);

        AddNewObjectToListOfExistingObjects(newObject, resourceType);

        newObject.SetActive(false);
    }

    private void AddNewObjectToListOfExistingObjects(GameObject newObject, ResourceTypes resourceType)
    {
        switch (resourceType)
        {
            case ResourceTypes.CROP:
                listOfCrops.Add(newObject);
                break;
            case ResourceTypes.PART:
                listOfParts.Add(newObject);
                break;
            case ResourceTypes.NITROGEN:
                listOfNitrogen.Add(newObject);
                break;
        }
    }

    #endregion
}
