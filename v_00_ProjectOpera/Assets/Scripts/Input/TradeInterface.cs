using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeInterface : MonoBehaviour
{
    [SerializeField] public GameObject planetPrefab;

    public void OnClickSellCrops()
    {
        if(GameManager.Instance.PlayerCrops > 0)
        {
            GameManager.Instance.TakeCropsFromPlayer(1);
            GameManager.Instance.AddCreditsToPlayer(5);
        }
    }

    public void OnClickSellParts()
    {
        if(GameManager.Instance.PlayerParts > 0)
        {
            GameManager.Instance.TakePartsFromPlayer(1);
            GameManager.Instance.AddCreditsToPlayer(5);
        }
    }

    public void OnClickSellNitrogen()
    {
        if(GameManager.Instance.PlayerNitrogen > 0)
        {
            GameManager.Instance.TakeNitrogenFromPlayer(1);
            GameManager.Instance.AddCreditsToPlayer(5);
        }
    }

    public void OnClickCreateAPlanet()
    {
        Planet oldPlanet = FindObjectOfType<Planet>();
        if (oldPlanet != null)
        {
            oldPlanet.DestroyPlanet();
        }

        // Instantiate a new planet
        GameObject newPlanetObject = Instantiate(planetPrefab);
        Planet newPlanet = newPlanetObject.GetComponent<Planet>();
        if (newPlanet != null)
        {
            newPlanet.GenerateRandomPlanet();
        }
    }
}
