using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUIHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI creditsDisplay;

    private void Awake()
    {
        creditsDisplay.text = GameManager.Instance.PlayerCredits.ToString() + " CREDITS";
    }

    public void OnClickMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
