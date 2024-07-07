using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineUI : MonoBehaviour
{
    // private int prodStepValue = 24;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI efficiencyLevelNumberLabel;
    [SerializeField] private TextMeshProUGUI inventoryNumberLabel;
    [SerializeField] private TextMeshProUGUI errorMessageLabel;
    [SerializeField] private TextMeshProUGUI updateButtonCostLabel;
    [SerializeField] Slider progressBarSlider;
    [SerializeField] Slider durabilityBarSlider;

    private void OnEnable()
    {
        progressBarSlider.value = 0;
    }
    public void SetSliderMaxValue(float maxValue)
    {
        progressBarSlider.maxValue = maxValue;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public void UpdateEfficiencyLevelText(int level)
    {
        efficiencyLevelNumberLabel.text = $"{level}/6";
    }

    public void UpdateInventoryLabel(int currentInventory, int maximumInventory)
    {
        string newLabel = $"{currentInventory}/{maximumInventory}";
        inventoryNumberLabel.text = newLabel;
    }

    public void UpdateButtonCostLabel(int cost)
    {
        updateButtonCostLabel.text = $"{cost} CREDITS";
    }
    
    public void UpdateErrorMessage(string error)
    {
        switch (error)
        {
            case "CLEAR":
                errorMessageLabel.text = "";
                errorMessageLabel.color = Color.white;
                break;
            case "BROKEN":
                errorMessageLabel.text = "BROKEN";
                errorMessageLabel.color = Color.red;
                Debug.Log("MachineUI: UpdateErrorMessage: error message set to broken.");
                break;
            case "EMPTY":
                errorMessageLabel.text = "EMPTY";
                errorMessageLabel.color = Color.yellow;
                break;
            default:
                errorMessageLabel.text = "";
                Debug.Log("MachineUI: UpdateErrorMessage: Machine failed to update error message");
                break;
        }
    }

    public void UpdateDurabilityBar(int durability)
    {
        Debug.Log("MachineUI: UpdateDurabilityBar: slider updated.");
        durabilityBarSlider.value = durability;
    }

    public void StartBarAnimation(int outputInterval)
    {
        StartCoroutine(AnimateProductionBar(outputInterval));
    }

    private IEnumerator AnimateProductionBar(int outputInterval)
    {

        float animationTime = 0f;
        while (animationTime < outputInterval)
        {
            animationTime += Time.deltaTime;
            float lerpValue = animationTime / outputInterval;
            progressBarSlider.value = Mathf.Lerp(0, progressBarSlider.maxValue, lerpValue);
            yield return null;
        }
        progressBarSlider.value = 0;
    }
}
