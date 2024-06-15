using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropMachine : MonoBehaviour
{

    [SerializeField] private int resourceInput;
    [SerializeField] private float timer;
    [SerializeField] private ObjectPooler objPooler;
    [SerializeField] private Transform outputPos;

    private Coroutine productionCoroutine;

    private void OnEnable()
    {
        objPooler = GameObject.Find("ObjectPooler").GetComponent<ObjectPooler>();
        productionCoroutine = StartCoroutine(Production(resourceInput, timer));
    }

    private void OnDisable()
    {
        StopCoroutine(productionCoroutine);
    }

    private IEnumerator Production(int input, float interval)
    {
        GameObject crop;

        while (true)
        {
            if (input > 0)
            {
                resourceInput--;
                input--;
                crop = objPooler.ReturnCrop();
                crop.transform.position = outputPos.position;
                crop.SetActive(true);
            }
            else
            {
                input = resourceInput;
            }
            yield return new WaitForSeconds(interval);
        }
    }

    //called to add resources to machine
    public void AddInput()
    {
        GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.NITROGEN);
        resourceInput++;
    }


    //called when upgrade is purchased and applied
    public void Upgrade(float reduction)
    {
        timer -= reduction;
    }
}
