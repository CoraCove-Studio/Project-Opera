using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeInterface : MonoBehaviour
{
    private int baseCropValue = 8;
    private int basePartValue = 18;
    private int baseNitrogenValue = 14;

    private int valueOfCrop;
    private int valueOfPart;
    private int valueOfNitrogen;

    [SerializeField] List<GameObject> planets;
    private int round = 1;
    private GameObject activePlanet;

    private void Start()
    {
        activePlanet = planets[0];
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TagManager.TRADING_HUB))
        {
            valueOfCrop = baseCropValue * other.GetComponent<PlanetTradingHub>().cropPriceBoost;
            valueOfPart = basePartValue * other.GetComponent<PlanetTradingHub>().partPriceBoost;
            valueOfNitrogen = baseNitrogenValue * other.GetComponent <PlanetTradingHub>().nitrogenPriceBoost;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(TagManager.TRADING_HUB))
        {
            Debug.Log("TradeInterface: OnTriggerExit: round completed");
            ResetValueOfProducts();
            round++;
            if (round < 3) { SetActivePlanet(round); }
        }
    }

    private void ResetValueOfProducts()
    {
        valueOfCrop = baseCropValue;
        valueOfPart = basePartValue;
        valueOfNitrogen= baseNitrogenValue;
    }

    private void SetActivePlanet(int round)
    {
        switch (round)
        {
            case 1:
                activePlanet.SetActive(false);
                activePlanet = planets[0];
                activePlanet.SetActive(true);
                break;
            case 2:
                activePlanet.SetActive(false);
                activePlanet = planets[1];
                activePlanet.SetActive(true);
                break;
            case 3:
                activePlanet.SetActive(false);
                activePlanet = planets[2];
                activePlanet.SetActive(true);
                break;
            default:
                Debug.Log("TradeInterface: SetActivePlanet: Error setting active planet");
                break;
        }
    }


    public void OnClickSellCrops()
    {
        if(GameManager.Instance.PlayerCrops > 0)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.CROP);
            GameManager.Instance.AddCreditsToPlayer(valueOfCrop);
        }
    }

    public void OnClickSellParts()
    {
        if(GameManager.Instance.PlayerParts > 0)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.PART);
            GameManager.Instance.AddCreditsToPlayer(valueOfPart);
        }
    }

    public void OnClickSellNitrogen()
    {
        if(GameManager.Instance.PlayerNitrogen > 0)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.NITROGEN);
            GameManager.Instance.AddCreditsToPlayer(valueOfNitrogen);
        }
    }
}
