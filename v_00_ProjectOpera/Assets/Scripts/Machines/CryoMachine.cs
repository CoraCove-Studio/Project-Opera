using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryoMachine : MonoBehaviour
{

    [SerializeField] private int resourceInput;
    [SerializeField] private float timer;
    [SerializeField] private ObjectPooler objPooler;
    [SerializeField] private Transform outputPos;
    private IEnumerator productionCoroutine;

    private void OnEnable()
    {
        productionCoroutine = Production(resourceInput, timer);
        StartCoroutine(productionCoroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(productionCoroutine);
    }

    private IEnumerator Production(int input, float interval)
    {
        GameObject nitrogen;

        while (true)
        {
            if (input > 0)
            {
                resourceInput--;
                input--;
                nitrogen = objPooler.ReturnNitrogen();
                nitrogen.transform.position = outputPos.position;
                nitrogen.SetActive(true);
            }
            else
            {
                input = resourceInput;
            }
            yield return new WaitForSeconds(interval);
        }
    }

    //called to add resources to machine
    public void AddInput(int addedResource)
    {
        resourceInput += addedResource;
    }


    //called when upgrade is purchased and applied
    public void Upgrade(float reduction)
    {
        timer -= reduction;
    }
}
