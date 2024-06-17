using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonitorInterface : MonoBehaviour
{
    [SerializeField] private GameObject monitorBody;

    [Header("UI Elements")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject yesButton;
    [SerializeField] private GameObject noButton;
    [SerializeField] private GameObject debugStartButton;

    private float textSpeed = 0.05f;
    private int currentTutTextIndex = 0;
    private int endTutorialTextIndex;

    private TutorialText tutText;

    private void Start()
    {
        tutText = new();
        endTutorialTextIndex = tutText.tutorialSteps.Length - 1;
        StartCoroutine(TypeTextEffect(GetNextTutText()));
#if UNITY_EDITOR
        debugStartButton.SetActive(true);
#endif
    }

    private string GetNextTutText()
    {
        var thisString = "error";
        if (currentTutTextIndex < tutText.tutorialSteps.Length)
        {
            thisString = tutText.tutorialSteps[currentTutTextIndex];
            currentTutTextIndex++;
        }
        return thisString;
    }

    public void OnClickStartButton()
    {
        GameManager.Instance.StartGame();
        InputManager.Instance.UnpauseWithButton();
    }

    public void OnClickNextButton()
    {
        nextButton.SetActive(false);
        if (currentTutTextIndex == 4)
        {
            yesButton.SetActive(false);
            noButton.SetActive(false);
        }
        StartCoroutine(TypeTextEffect(GetNextTutText()));
    }

    public void OnClickNoButton()
    {
        yesButton.SetActive(false);
        noButton.SetActive(false);
        currentTutTextIndex = endTutorialTextIndex;
        StartCoroutine(TypeTextEffect(GetNextTutText()));
    }

    private void UpdateTextField(string text)
    {
        textField.text = text;
    }

    private IEnumerator TypeTextEffect(string textToType)
    {
        char[] charArray = textToType.ToCharArray();
        List<char> outputCharList = new();
        for (int i = 0; i < charArray.Length; i++)
        {
            outputCharList.Add(charArray[i]);
            UpdateTextField(new string(outputCharList.ToArray())); // Convert List<char> to string
            yield return new WaitForSeconds(textSpeed);
        }

        ActivateAppropriateButtons();
    }

    private void ActivateAppropriateButtons()
    {
        if (currentTutTextIndex < 4 || currentTutTextIndex > 4 && currentTutTextIndex < tutText.tutorialSteps.Length - 1)
        {
            nextButton.SetActive(true);
        }
        else if (currentTutTextIndex == 4)
        {
            yesButton.SetActive(true);
            noButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(true);
        }

    }
}
