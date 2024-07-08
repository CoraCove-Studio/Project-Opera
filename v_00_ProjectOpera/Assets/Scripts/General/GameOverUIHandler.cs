using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUIHandler : MonoBehaviour
{
    [Header("Number Labels")]
    [SerializeField] private TextMeshProUGUI debtNumberLabel;
    [SerializeField] private TextMeshProUGUI netProfitNumberLabel;
    [SerializeField] private TextMeshProUGUI creditsEarnedNumberLabel;
    [SerializeField] private TextMeshProUGUI creditsSpentNumberLabel;
    [SerializeField] private TextMeshProUGUI machinesBrokenNumberLabel;
    [SerializeField] private TextMeshProUGUI itemsProducedNumberLabel;
    [SerializeField] private TextMeshProUGUI itemsCollectedNumberLabel;
    [SerializeField] private TextMeshProUGUI resultsLabel;
    [SerializeField] private TextMeshProUGUI returnButtonLabel;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        UpdateStatistics(gameManager.playerStatistics);
        SetResultsLabelText();
    }

    private void Awake()
    {
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

    private void SetResultsLabelText()
    {
        if(gameManager.PlayerDebt <= 0)
        {
            resultsLabel.text = "GREAT WORK! YOU PAID OFF YOUR DEBT!";
            returnButtonLabel.text = "NEW JOURNEY";
        }
        else if(gameManager.PlayerDebt > 0)
        {
            resultsLabel.text = "YOU DID NOT EARN ENOUGH TO PAY OFF YOUR DEBT";
            returnButtonLabel.text = "SELL SHIP";
        }
    }
    public void OnClickReturn()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
