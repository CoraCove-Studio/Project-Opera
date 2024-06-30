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
    [SerializeField] List<AudioClip> sellingNoises;
    private int round = 1;
    private GameObject activePlanet;

    private void Start()
    {
        activePlanet = planets[0];
        ResetValueOfProducts();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TagManager.TRADING_HUB))
        {
            valueOfCrop = baseCropValue * other.GetComponent<PlanetTradingHub>().cropPriceBoost;
            valueOfPart = basePartValue * other.GetComponent<PlanetTradingHub>().partPriceBoost;
            valueOfNitrogen = baseNitrogenValue * other.GetComponent <PlanetTradingHub>().nitrogenPriceBoost;
        }
        else if (other.gameObject.CompareTag(TagManager.WARP_SPEED_ACTIVATOR))
        {
            Debug.Log("TradeInterface: OnTriggerEnter: Warp Speed Activated");
            //activate particle effects here
        }
        else if (other.gameObject.CompareTag(TagManager.WARP_SPEED_DEACTIVATOR))
        {
            Debug.Log("TradeInterface: OnTriggerEnter: Warp Speed Deactivated");
            //deactivate particle effects here
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

    private AudioClip GetRandomNoiseClip()
    {
        return sellingNoises[Random.Range(0, sellingNoises.Count)];
    }


    public void OnClickSellCrops()
    {
        if(GameManager.Instance.PlayerCrops > 0)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.CROP);
            GameManager.Instance.AddCreditsToPlayer(valueOfCrop);
            GameManager.Instance.audioManager.PlaySFX(GetRandomNoiseClip());
        }
    }

    public void OnClickSellParts()
    {
        if(GameManager.Instance.PlayerParts > 0)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.PART);
            GameManager.Instance.AddCreditsToPlayer(valueOfPart);
            GameManager.Instance.audioManager.PlaySFX(GetRandomNoiseClip());
        }
    }

    public void OnClickSellNitrogen()
    {
        if(GameManager.Instance.PlayerNitrogen > 0)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.NITROGEN);
            GameManager.Instance.AddCreditsToPlayer(valueOfNitrogen);
            GameManager.Instance.audioManager.PlaySFX(GetRandomNoiseClip());
        }
    }
}
