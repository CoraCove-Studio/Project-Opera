using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DebtInterface : MonoBehaviour
{
    [Header("Number Labels")]
    [SerializeField] private TextMeshProUGUI debtNumberLabel;
    [SerializeField] private TextMeshProUGUI netProfitNumberLabel;
    [SerializeField] private TextMeshProUGUI creditsEarnedNumberLabel;
    [SerializeField] private TextMeshProUGUI creditsSpentNumberLabel;
    [SerializeField] private TextMeshProUGUI machinesBrokenNumberLabel;
    [SerializeField] private TextMeshProUGUI itemsProducedNumberLabel;
    [SerializeField] private TextMeshProUGUI itemsCollectedNumberLabel;

    public void UpdateStatistics(Dictionary<string, int> statistics)
    {
        debtNumberLabel.text = statistics["Player Debt"].ToString();
        netProfitNumberLabel.text = statistics["Net Profit"].ToString();
        creditsEarnedNumberLabel.text = statistics["Credits Earned"].ToString();
        creditsSpentNumberLabel.text = statistics["Credits Spent"].ToString();
        machinesBrokenNumberLabel.text = statistics["Machines Broken"].ToString();
        itemsProducedNumberLabel.text = statistics["Items Produced"].ToString();
        itemsCollectedNumberLabel.text = statistics["Items Collected"].ToString();
    }

    public void OnClickMakePayment(int amount)
    {
        if(GameManager.Instance.PlayerCredits >= amount)
        {
            GameManager.Instance.TakeCreditsFromPlayer(amount);
            GameManager.Instance.PayDebt(amount);
        }
    }
}
