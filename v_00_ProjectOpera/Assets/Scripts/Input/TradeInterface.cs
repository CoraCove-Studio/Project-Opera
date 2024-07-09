using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TradeInterface : MonoBehaviour
{
    private int baseCropValue = 8;
    private int basePartValue = 18;
    private int baseNitrogenValue = 14;

    private int valueOfCrop;
    private int valueOfPart;
    private int valueOfNitrogen;

    [SerializeField] List<GameObject> planets;
    [SerializeField] List<GameObject> planetAtmospheres;
    [SerializeField] List<AudioClip> sellingNoises;
    private int round = 0;
    private GameObject activePlanet;
    [SerializeField] private GameObject activePlanetAtmosphere;

    [Header("Price Labels")]
    [SerializeField] private TextMeshProUGUI partPriceLabel;
    [SerializeField] private TextMeshProUGUI nitroPriceLabel;
    [SerializeField] private TextMeshProUGUI cropPriceLabel; 

    [Header("Buttons")]
    [SerializeField] private GameObject sellPartButton;
    [SerializeField] private GameObject sellCropButton;
    [SerializeField] private GameObject sellNitroButton;
    [SerializeField] private GameObject sellAllButton;

    [Header("Button Text")]
    [SerializeField] private GameObject priceButtonLabels;
    [SerializeField] private GameObject sellProductLabels;
    [SerializeField] private GameObject sellAllButtonLabel;

    [SerializeField] private AudioClip moneyNoise;
    private bool playerSoldItem = false;

    [Header("WarpSpeed VFX")]
    [SerializeField] private GameObject warpSpeedVFX;

    private void Start()
    {
        SetActivePlanet(round);
        SetActivePlanetAtmosphere(round);
        ResetValueOfProducts();
        UpdatePriceLabels();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TagManager.TRADING_HUB))
        {
            GameManager.Instance.PlayerUI.ActivateTicker();
            if (GameManager.Instance.InTutorial == false) GameManager.Instance.PlayerUI.SendTimedNotification("Trade now!");
            var tradingHub = other.GetComponent<PlanetTradingHub>();
            valueOfCrop = baseCropValue * tradingHub.cropPriceBoost;
            valueOfPart = basePartValue * tradingHub.partPriceBoost;
            valueOfNitrogen = baseNitrogenValue * tradingHub.nitrogenPriceBoost;
            UpdatePriceLabels();
            if(round == 2)
            {
                DeactivateSellingButtons();
                ActivateSellAllButton();
            }
        }
        else if (other.gameObject.CompareTag(TagManager.WARP_SPEED_ACTIVATOR))
        {
            Debug.Log("TradeInterface: OnTriggerEnter: Warp Speed Activated.");
            GameManager.Instance.PlayerUI.SendTimedNotification("Entering warp speed!");
            activePlanetAtmosphere.SetActive(false);
            warpSpeedVFX.GetComponent<WarpSpeed>().EnteringWarp();
            //activate particle effects here
        }
        else if (other.gameObject.CompareTag(TagManager.WARP_SPEED_DEACTIVATOR))
        {
            Debug.Log("TradeInterface: OnTriggerEnter: Warp Speed Deactivated");
            GameManager.Instance.PlayerUI.SendTimedNotification("Exiting warp speed.");
            //deactivate particle effects here

            warpSpeedVFX.GetComponent<WarpSpeed>().LeavingWarp();
            if (round <= 3) { SetActivePlanetAtmosphere(round); }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(TagManager.TRADING_HUB))
        {
            Debug.Log("TradeInterface: OnTriggerExit: round completed");
            ResetValueOfProducts();
            if (!GameManager.Instance.InTutorial) round++;
            UpdatePriceLabels();
            GameManager.Instance.PlayerUI.ActivateTicker();
        }
        else if (other.gameObject.CompareTag(TagManager.WARP_SPEED_DEACTIVATOR))
        {
            if (round == 2)
            {
                GameManager.Instance.PlayerUI.SendTimedNotification("Last planet incoming!");
            }
            else
            {
                GameManager.Instance.PlayerUI.SendTimedNotification("Planet incoming!");
            }
        }
        else if (other.gameObject.CompareTag(TagManager.WARP_SPEED_ACTIVATOR))
        {
            if (round <= 3) { SetActivePlanet(round); }
        }
    }

    private void UpdatePriceLabels()
    {
        partPriceLabel.text = $"{valueOfPart} CREDITS";
        nitroPriceLabel.text = $"{valueOfNitrogen} CREDITS";
        cropPriceLabel.text = $"{valueOfCrop} CREDITS";
    }

    private void ActivateSellAllButton()
    {
        sellAllButton.SetActive(true);
        sellAllButtonLabel.SetActive(true);
    }

    private void DeactivateSellingButtons()
    {
        sellCropButton.SetActive(false);
        sellNitroButton.SetActive(false);
        sellPartButton.SetActive(false);

        priceButtonLabels.SetActive(false);
        sellProductLabels.SetActive(false);
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
            case 0:
                activePlanet = planets[0];
                break;
            case 1:
                activePlanet.SetActive(false);
                activePlanet = planets[1];
                activePlanet.SetActive(true);
                break;
            case 2:
                activePlanet.SetActive(false);
                activePlanet = planets[2];
                activePlanet.SetActive(true);
                break;
            default:
                Debug.Log("TradeInterface: SetActivePlanet: Error setting active planet");
                break;
        }
    }

    private void SetActivePlanetAtmosphere(int round)
    {
        switch (round)
        {
            case 0:
                activePlanetAtmosphere = planetAtmospheres[0];
                activePlanetAtmosphere.SetActive(true);
                Debug.Log("TradeInterface: SetActivePlanet: Set first atmosphere active");
                break;
            case 1:
                activePlanetAtmosphere.SetActive(false);
                activePlanetAtmosphere = planetAtmospheres[1];
                Debug.Log("TradeInterface: SetActivePlanet: Set first atmosphere active");
                activePlanetAtmosphere.SetActive(true);
                break;
            case 2:
                activePlanetAtmosphere.SetActive(false);
                activePlanetAtmosphere = planetAtmospheres[2];
                Debug.Log("TradeInterface: SetActivePlanet: Set second atmosphere active");
                activePlanetAtmosphere.SetActive(true);
                break;
            case 3:
                activePlanetAtmosphere.SetActive(false);
                activePlanetAtmosphere = planetAtmospheres[2];
                Debug.Log("TradeInterface: SetActivePlanet: Set third atmosphere active");
                activePlanetAtmosphere.SetActive(true);
                break;
            default:
                Debug.Log("TradeInterface: SetActivePlanet: Error setting active atmosphere");
                break;
        }
    }

    private AudioClip GetRandomNoiseClip()
    {
        return sellingNoises[Random.Range(0, sellingNoises.Count)];
    }


    public void OnClickSellCrops()
    {
        if(GameManager.Instance.PlayerCrops > 1)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.CROP);
            GameManager.Instance.AddCreditsToPlayer(valueOfCrop);
            GameManager.Instance.audioManager.PlaySFX(GetRandomNoiseClip());
            playerSoldItem = true;
        }
        else
        {
            GameManager.Instance.audioManager.PlayErrorNoise();
        }
    }

    public void OnClickSellParts()
    {
        if(GameManager.Instance.PlayerParts > 1)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.PART);
            GameManager.Instance.AddCreditsToPlayer(valueOfPart);
            GameManager.Instance.audioManager.PlaySFX(GetRandomNoiseClip());
            playerSoldItem = true;
        }
        else
        {
            GameManager.Instance.audioManager.PlayErrorNoise();
        }
    }

    public void OnClickSellNitrogen()
    {
        if(GameManager.Instance.PlayerNitrogen > 1)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, ResourceTypes.NITROGEN);
            GameManager.Instance.AddCreditsToPlayer(valueOfNitrogen);
            GameManager.Instance.audioManager.PlaySFX(GetRandomNoiseClip());
            playerSoldItem = true;
        }
        else
        {
            GameManager.Instance.audioManager.PlayErrorNoise();
        }
    }

    public void OnClickSellAll()
    {
        int value = (GameManager.Instance.PlayerCrops * valueOfCrop) + (GameManager.Instance.PlayerNitrogen * valueOfNitrogen) + (GameManager.Instance.PlayerParts * valueOfPart);
        GameManager.Instance.AddCreditsToPlayer(value);

        GameManager.Instance.TakeResourceFromPlayer(GameManager.Instance.PlayerCrops, ResourceTypes.CROP);
        GameManager.Instance.TakeResourceFromPlayer(GameManager.Instance.PlayerNitrogen, ResourceTypes.NITROGEN);
        GameManager.Instance.TakeResourceFromPlayer(GameManager.Instance.PlayerParts, ResourceTypes.PART);
        GameManager.Instance.audioManager.PlaySFX(moneyNoise);
    }

    public void PlayerLeftCollider()
    {
        if (playerSoldItem)
        {
            GameManager.Instance.audioManager.PlaySFX(moneyNoise);
        }
        playerSoldItem = false;
    }
}
