using System.Collections;
using TMPro;
using UnityEngine;

public class MachineUI : MonoBehaviour
{
    // private int prodStepValue = 24;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI levelNumberLabel;
    [SerializeField] private TextMeshProUGUI inventoryNumberLabel;
    [SerializeField] private GameObject progressBar;

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public void UpdateLevelText(int level)
    {
        levelNumberLabel.text = level.ToString();
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
        Vector3 fullBar = new(1, 0, 0);
        float elapsedTime = 0f;
        Debug.Log("AnimateProductionBar: coroutine started.");

        while (elapsedTime < outputInterval)
        {
            Debug.Log("AnimateProductionBar: inside of while loop.");
            progressBar.transform.localScale = Vector3.Lerp(progressBar.transform.localScale, fullBar, outputInterval);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }
}
