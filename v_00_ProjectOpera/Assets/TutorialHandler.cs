using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    private GameManager gameManager;
    private float checkInterval = 0.2f;

    [SerializeField] private List<Collider> doorColliders; // index 0 is the main door
    [SerializeField] private List<GameObject> machineSlots;

    [SerializeField] private Dictionary<string, bool> conditions = new()
{
    { "PlacedPartsMachine",             false },
    { "PlacedNitrogenMachine",          false },
    { "PlacedCropsMachine",             false },

    { "FiveCropsInPartsMachine",        false },
    { "FivePartsInNitrogenMachine",     false },
    { "FiveNitrogenInCropsMachine",     false },

    { "RepairedMachine",                false },
    { "UpgradedMachine",                false },

    { "1MinutePassed",                  false },

    { "5SecondsPassed",                 false },

    { "Made600Credits",                 false },
    { "MadeDebtPayment",                false }
};


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void SetUpTutorial()
    {
        ToggleAllDoors(false);
        Debug.Log("TutorialHandler: SetUpTutorial: Doors turned off.");

        // Turn all but one of the machine slot interactable componenets off
        for (int i = 1; i < machineSlots.Count; i++)
        {
            machineSlots[i].SetActive(false);
            Debug.Log("TutorialHandler: SetUpTutorial: MachineSlots turned off.");
        }

        // place arrow indicator over the one left?

    }

    public void StartTutorialCoroutine()
    {
        Debug.Log("Trying to start tutorial coroutine.");
        StartCoroutine(TutorialCoroutine());
    }

    private IEnumerator TutorialCoroutine()
    {
        Debug.Log("Tutorial coroutine started.");
        while (gameManager.InTutorial)
        {
            Debug.Log("Tutorial started.");
            SetUpTutorial();

            // Set floating blue indicator
            gameManager.PlayerUI.SendConditionalNotification("Place a printer!");

            while (conditions["PlacedPartsMachine"] == false)
            {
                yield return new WaitForSeconds(checkInterval);
            }

            Debug.Log("Tutorialhandler: Main Routine: Parts machine was placed.");
            gameManager.PlayerUI.CloseConditionalNotification();

            machineSlots[1].SetActive(true);

            gameManager.PlayerUI.SendConditionalNotification("Place a cryopod!");

            while (conditions["PlacedNitrogenMachine"] == false)
            {
                yield return new WaitForSeconds(checkInterval);
            }

            Debug.Log("Tutorialhandler: Main Routine: Nitrogen machine was placed.");
            gameManager.PlayerUI.CloseConditionalNotification();

            gameManager.StartGameFromTutorial(); // Sets InTutorial to false
        }
    }

    private void ToggleAllDoors(bool unlocked)
    {
        foreach (Collider doorCollider in doorColliders)
        {
            doorCollider.enabled = unlocked;
        }
    }

    #region Conditional Toggles
    public void PlacedPartsMachine()
    {
        conditions["PlacedPartsMachine"] = true;
    }

    public void PlacedNitrogenMachine()
    {
        conditions["PlacedNitrogenMachine"] = true;
    }

    public void PlacedCropsMachine()
    {
        conditions["PlacedCropsMachine"] = true;
    }

    public void FiveCropsInPartsMachine()
    {
        conditions["FiveCropsInPartsMachine"] = true;
    }

    public void FivePartsInNitrogenMachine()
    {
        conditions["FivePartsInNitrogenMachine"] = true;
    }

    public void FiveNitrogenInCropsMachine()
    {
        conditions["FiveNitrogenInCropsMachine"] = true;
    }

    public void RepairedMachine()
    {
        conditions["RepairedMachine"] = true;
    }

    public void UpgradedMachine()
    {
        conditions["UpgradedMachine"] = true;
    }

    public void OneMinutePassed()
    {
        conditions["1MinutePassed"] = true;
    }

    public void FiveSecondsPassed()
    {
        conditions["5SecondsPassed"] = true;
    }

    public void Made600Credits()
    {
        conditions["Made600Credits"] = true;
    }

    public void MadeDebtPayment()
    {
        conditions["MadeDebtPayment"] = true;
    }

    #endregion
}
