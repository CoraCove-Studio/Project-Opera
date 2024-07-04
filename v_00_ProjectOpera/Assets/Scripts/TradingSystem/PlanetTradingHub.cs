using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetTradingHub : MonoBehaviour
{
    public int cropPriceBoost;
    public int partPriceBoost;
    public int nitrogenPriceBoost;

    public void SetNewBoostValues()
    {
        cropPriceBoost = Random.Range(1, 8);
        partPriceBoost = Random.Range(1, 8);
        nitrogenPriceBoost = Random.Range(1, 8);
    }
}
