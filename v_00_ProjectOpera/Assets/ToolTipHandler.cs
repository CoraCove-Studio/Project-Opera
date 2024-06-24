using System.Collections;
using TMPro;
using UnityEngine;
using Unity.UI;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToolTipHandler : MonoBehaviour
{
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private CanvasGroup priceDisplay;
    [SerializeField] private CanvasGroup resourceDisplay;
    [SerializeField] private TextMeshProUGUI priceLabel;
    [SerializeField] private TextMeshProUGUI amountLabel;
    [SerializeField] private Image resourceImage;
    [SerializeField] private List<Sprite> listOfResourceSprites = new();

    private CanvasGroup currentDisplay;
    private bool currentlyDisplaying = false;

    public void DisplayValue(int value)
    {
        priceLabel.text = value.ToString();
        if (currentlyDisplaying == false)
        {
            currentDisplay = priceDisplay;
            StartCoroutine(ActivateDisplay(currentDisplay));
        }
        else if (currentlyDisplaying && currentDisplay != priceDisplay)
        {
            HotSwitchDisplay();
        }
    }

    public void DisplayResource(ResourceTypes type, int value)
    {
        amountLabel.text = value.ToString();
        SetNewResourceImage(type);
        if (currentlyDisplaying == false)
        {
            amountLabel.text = value.ToString();
            currentDisplay = resourceDisplay;
            StartCoroutine(ActivateDisplay(currentDisplay));
        }
        else if (currentlyDisplaying && currentDisplay != resourceDisplay)
        {
            HotSwitchDisplay();
        }
    }

    private void HotSwitchDisplay()
    {
        currentDisplay.gameObject.SetActive(false);
        if (currentDisplay == priceDisplay)
        {
            resourceDisplay.alpha = 1f;
            currentDisplay = resourceDisplay;
        }
        else if (currentDisplay == resourceDisplay)
        {
            priceDisplay.alpha = 1f;
            currentDisplay = priceDisplay;
        }
        currentDisplay.gameObject.SetActive(true);
    }

    public void ClearDisplay()
    {
        StopAllCoroutines();
        StartCoroutine(DeactivateDisplay());
    }

    private IEnumerator ActivateDisplay(CanvasGroup display)
    {
        currentlyDisplaying = true;
        display.gameObject.SetActive(true);
        display.alpha = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            display.alpha = Mathf.Clamp01(elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        display.alpha = 1f;
    }

    private IEnumerator DeactivateDisplay()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            currentDisplay.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currentDisplay.alpha = 0f;
        currentDisplay.gameObject.SetActive(false);
        currentlyDisplaying = false;
    }

    public void EnableTooltipPanel()
    {
        tooltipPanel.SetActive(true);
    }

    public void DisableTooltipPanel()
    {
        tooltipPanel.SetActive(false);
    }

    private void SetNewResourceImage(ResourceTypes resourceType)
    {
        switch (resourceType)
        {
            case ResourceTypes.CROP:
                resourceImage.sprite = listOfResourceSprites[0];
                break;
            case ResourceTypes.PART:
                resourceImage.sprite = listOfResourceSprites[1];
                break;
            case ResourceTypes.NITROGEN:
                resourceImage.sprite = listOfResourceSprites[2];
                break;
            default:
                Debug.Log("ToolTipHandler: SetNewResourceImage: Default switch case. Image not changed.");
                break;
        }
    }
}
