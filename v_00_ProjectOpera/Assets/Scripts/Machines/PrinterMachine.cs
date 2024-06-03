using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterMachine : MonoBehaviour
{

    [SerializeField] private int resourceInput;
    [SerializeField] private float timer;
    [SerializeField] private ObjectPooler objPooler;
    [SerializeField] private Transform outputPos;

    private void OnEnable()
    {
        StartCoroutine(Production(resourceInput, timer));
    }

    private void OnDisable()
    {
        StopCoroutine(Production(resourceInput, timer));
    }


    //only for testing if AddInput works
    //private void Update()
    //{
    //    if (resourceInput <= 0)
    //    {
    //        AddInput(10);
    //    }
    //}
    private IEnumerator Production(int input, float interval)
    {
        GameObject part;
        while (input > 0)
        {
            yield return new WaitForSeconds(interval);
            resourceInput--;
            part = objPooler.ReturnPart();
            part.transform.position = outputPos.position;
            part.SetActive(true);
        }
        input = resourceInput;
        interval = timer;
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
