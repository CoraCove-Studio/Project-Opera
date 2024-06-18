using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineUI : MonoBehaviour
{
    // private int prodStepValue = 24;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI efficiencyLevelNumberLabel;
    [SerializeField] private TextMeshProUGUI outputLevelNumberLabel;
    [SerializeField] private TextMeshProUGUI inventoryNumberLabel;
    [SerializeField] Slider progressBarSlider;

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
        efficiencyLevelNumberLabel.text = level.ToString();
    }

    public void UpdateOutputIntervalLevelText(int level)
    {
        outputLevelNumberLabel.text = level.ToString();
    }

    public void UpdateInventoryLabel(int currentInventory, int maximumInventory)
    {
        string newLabel = $"{currentInventory}/{maximumInventory}";
        inventoryNumberLabel.text = newLabel;
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
