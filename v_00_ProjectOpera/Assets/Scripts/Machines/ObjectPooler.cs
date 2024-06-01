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

    private GameObject GetNewCrop()
    {
        GameObject crop;

            crop = Instantiate(listOfCropPrefabs[0]);
            Crop _ = crop.GetComponent<Crop>();
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
        part = ReturnInactiveObject(listOfCrops);

        if (part == null)
        {
            part = GetNewPart();
        }
        return part;
    }

    private GameObject GetNewPart()
    {
        GameObject part;

        part = Instantiate(listOfPartPrefabs[0]);
        Part _ = part.GetComponent<Part>();
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

    private GameObject GetNewNitrogen()
    {
        GameObject nitrogen;

        nitrogen = Instantiate(listOfNitrogenPrefabs[0]);
        Nitrogen _ = nitrogen.GetComponent<Nitrogen>();
        _.SetObjectPoolerReference(this);
        listOfNitrogen.Add(nitrogen);

        nitrogen.SetActive(false);
        return nitrogen;
    }
    #endregion
}
