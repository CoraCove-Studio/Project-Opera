using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    ObjectPooler objectPooler;

    public void SetObjectPoolerReference(ObjectPooler reference)
    {
        objectPooler = reference;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TagManager.PLAYER))
        {
            gameObject.transform.parent = objectPooler.transform;
            gameObject.SetActive(false);
        }
    }

}
