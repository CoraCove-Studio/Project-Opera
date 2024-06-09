using System.Collections;
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
    private GameObject ReturnInactiveObject(List<GameObject> listOfObjects)
    {
        foreach (GameObject gameObject in listOfObjects)
        {
            if( gameObject.activeInHierarchy == false)
            {
                return gameObject;
            }
        }

        return null;
    }
    #endregion

    #region crop methods

    public GameObject ReturnCrop()
    {
        GameObject crop;
        crop = ReturnInactiveObject(listOfCrops);

        if (crop == null)
        {
            crop = GetNewCrop();
        }
        return crop;
    }

    //instantiates new crop if no crops inactive in hierarchy
    private GameObject GetNewCrop()
    {
        GameObject crop;

            crop = Instantiate(listOfCropPrefabs[0]);
            Product _ = crop.GetComponent<Product>();
            _.SetObjectPoolerReference(this);
            listOfCrops.Add(crop);

        crop.SetActive(false);
        return crop;
    }
    #endregion

    #region parts methods
    public GameObject ReturnPart()
    {
        GameObject part;
        part = ReturnInactiveObject(listOfParts);

        if (part == null)
        {
            part = GetNewPart();
        }
        return part;
    }

    //instantiates new part if no parts inactive in hierarchy
    private GameObject GetNewPart()
    {
        GameObject part;

        part = Instantiate(listOfPartPrefabs[0]);
        Product _ = part.GetComponent<Product>();
        _.SetObjectPoolerReference(this);
        listOfParts.Add(part);

        part.SetActive(false);
        return part;
    }
    #endregion

    #region nitrogen methods
    public GameObject ReturnNitrogen()
    {
        GameObject nitrogen;
        nitrogen = ReturnInactiveObject(listOfCrops);

        if (nitrogen == null)
        {
            nitrogen = GetNewNitrogen();
        }
        return nitrogen;
    }

    //instantiates new nitrogen if no nitrogen inactive in hierarchy
    private GameObject GetNewNitrogen()
    {
        GameObject nitrogen;

        nitrogen = Instantiate(listOfNitrogenPrefabs[0]);
        Product _ = nitrogen.GetComponent<Product>();
        _.SetObjectPoolerReference(this);
        listOfNitrogen.Add(nitrogen);

        nitrogen.SetActive(false);
        return nitrogen;
    }
    #endregion
}
