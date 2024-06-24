using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingSystem : MonoBehaviour
{
    [SerializeField] public int priceOfCrop;
    [SerializeField] public int priceOfPart;
    [SerializeField] public int priceOfNitrogen;

    [SerializeField] private List<GameObject> planets = new List<GameObject>();
    [SerializeField] private int round;

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

    }

    private void DetermineProductValues()
    {

    }

    private void SetCurrentPlanetActive()
    {

    }
}
