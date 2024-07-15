using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebtInterface : MonoBehaviour
{
    [SerializeField] private GameObject debtUIActivator;
    [Header("Number Labels")]
    [SerializeField] private TextMeshProUGUI debtNumberLabel;
    [SerializeField] private TextMeshProUGUI netProfitNumberLabel;
    [SerializeField] private TextMeshProUGUI creditsEarnedNumberLabel;
    [SerializeField] private TextMeshProUGUI creditsSpentNumberLabel;
    [SerializeField] private TextMeshProUGUI machinesBrokenNumberLabel;
    [SerializeField] private TextMeshProUGUI itemsProducedNumberLabel;
    [SerializeField] private TextMeshProUGUI itemsCollectedNumberLabel;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void ActivateDebtUI()
    {
        debtUIActivator.SetActive(true);
    }

    public void DeactivateDebtUI()
    {
        debtUIActivator.SetActive(false);
    }
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
        bool madePayment = false;
        if (gameManager.InTutorial)
        {
            gameManager.PayDebt(amount);
            gameManager.TakeCreditsFromPlayer(amount);
            gameManager.TutorialHandler.MadeDebtPayment();
            madePayment = true;
        }
        else
        {

            if (gameManager.PlayerCredits >= amount && gameManager.PlayerDebt >= amount && gameManager.PlayerDebt > 0)
            {
                gameManager.PayDebt(amount);
                gameManager.TakeCreditsFromPlayer(amount);
                madePayment = true;
            }
            else if (gameManager.PlayerCredits >= gameManager.PlayerDebt && gameManager.PlayerDebt < amount && gameManager.PlayerDebt > 0)
            {
                OnClickPayAllDebt();
                madePayment = true;
            }
        }
        UpdateStatistics(gameManager.playerStatistics);
        if (madePayment) gameManager.audioManager.PlayDebtPaymentNoise();

    }

    public void OnClickPayAllDebt()
    {
        if (gameManager.PlayerCredits >= gameManager.PlayerDebt && gameManager.PlayerDebt > 0)
        {
            Debug.Log("DebtInterface: OnClickPayAllDebt: debt paid.");
            gameManager.TakeCreditsFromPlayer(gameManager.PlayerDebt);
            gameManager.PayDebt(gameManager.PlayerDebt);
            gameManager.audioManager.PlayDebtPaymentNoise();
        }
    }
}
