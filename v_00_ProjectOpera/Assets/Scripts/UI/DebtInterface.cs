using UnityEngine;
using TMPro;

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

    public void UpdateStatistics()
    {
        debtNumberLabel.text = GameManager.Instance.playerStatistics["Player Debt"].ToString();
    }

    public void OnClickMakePayment()
    {
        
    }

    public void OnClickPayAllDebt()
    {

    }
}
