using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    private GameManager gameManager;
    private float checkInterval = 0.2f;

    private int cropsPlacedInMachine;
    private int partsPlacedInMachine;
    private int nitrogenPlacedInMachine;

    [SerializeField] private List<Collider> doorColliders; // index 0 is the main door
    [SerializeField] private List<GameObject> machineSlots;

    [SerializeField]
    private Dictionary<string, bool> conditions = new()
{
    { "PlacedPartsMachine",             false },
    { "PlacedNitrogenMachine",          false },
    { "PlacedCropsMachine",             false },

    { "OneCropInPartsMachine",          false },
    { "TwoPartsInNitrogenMachine",      false },
    { "SixNitrogenInCropsMachine",      false },

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
        StartCoroutine(TutorialCoroutine());
    }

    private IEnumerator TutorialCoroutine()
    {
        while (gameManager.InTutorial)
        {
            SetUpTutorial();

            #region Placing Printer Machine
            gameManager.PlayerUI.SendConditionalNotification("Place a printer!");
            while (conditions["PlacedPartsMachine"] == false)
            {
                yield return new WaitForSeconds(checkInterval);
            }
            Debug.Log("Tutorialhandler: Main Routine: Parts machine was placed.");
            gameManager.PlayerUI.CloseConditionalNotification();
            #endregion

            #region Placing Nitrogen Machine
            machineSlots[1].SetActive(true);


            gameManager.PlayerUI.SendConditionalNotification("Place a cryopod!");
            while (conditions["PlacedNitrogenMachine"] == false)
            {
                yield return new WaitForSeconds(checkInterval);
            }
            Debug.Log("Tutorialhandler: Main Routine: Nitrogen machine was placed.");
            gameManager.PlayerUI.CloseConditionalNotification();
            #endregion

            #region Placing Crop Machine
            machineSlots[2].SetActive(true);

            gameManager.PlayerUI.SendConditionalNotification("Place a greenhouse!");
            while (conditions["PlacedCropsMachine"] == false)
            {
                yield return new WaitForSeconds(checkInterval);
            }
            Debug.Log("Tutorialhandler: Main Routine: Crops machine was placed.");
            gameManager.PlayerUI.CloseConditionalNotification();
            #endregion

            #region Loading 1 Crop
            gameManager.PlayerUI.SendConditionalNotification("Place 1 crop into the printer!");
            while (conditions["OneCropInPartsMachine"] == false)
            {
                yield return new WaitForSeconds(checkInterval);
            }
            Debug.Log("Tutorialhandler: Main Routine: One crop put in parts machine.");
            gameManager.PlayerUI.CloseConditionalNotification();
            #endregion

            #region Loading 2 Parts
            gameManager.PlayerUI.SendConditionalNotification("Place 2 parts into the cryopod!");
            while (conditions["TwoPartsInNitrogenMachine"] == false)
            {
                yield return new WaitForSeconds(checkInterval);
            }
            Debug.Log("Tutorialhandler: Main Routine: Two parts put in nitrogen machine.");
            gameManager.PlayerUI.CloseConditionalNotification();
            #endregion

            #region Loading 6 Nitrogen
            gameManager.PlayerUI.SendConditionalNotification("Place 6 nitrogen into the greenhouse!");
            while (conditions["SixNitrogenInCropsMachine"] == false)
            {
                yield return new WaitForSeconds(checkInterval);
            }
            Debug.Log("Tutorialhandler: Main Routine: Six nitrogen put in crop machine.");
            gameManager.PlayerUI.CloseConditionalNotification();

            #endregion

            #region Repairing Machine
            machineSlots[0].GetComponent<MachineSlot>().SpawnedMachine.TryGetComponent(out MachineBehavior machine);
            machine.BreakMachine();

            gameManager.PlayerUI.SendTimedNotification("A machine broke!");
            gameManager.PlayerUI.SendConditionalNotification("Repair the machine!");

            while (conditions["RepairedMachine"] == false)
            {
                yield return new WaitForSeconds(checkInterval);
            }
            Debug.Log("Tutorialhandler: Main Routine: Repaired machine.");
            gameManager.PlayerUI.CloseConditionalNotification();
            #endregion

            #region Upgrading Machine
            gameManager.PlayerUI.SendConditionalNotification("Upgrade a machine!");
            while (conditions["UpgradedMachine"] == false)
            {
                yield return new WaitForSeconds(checkInterval);
            }
            Debug.Log("Tutorialhandler: Main Routine: Upgraded machine.");
            gameManager.PlayerUI.CloseConditionalNotification();
            #endregion

            #region Waiting For a Minute
            gameManager.PlayerUI.SendConditionalNotification("Work for a minute!");
            while (conditions["1MinutePassed"] == false)
            {
                // Ideally, start a visible game clock
                yield return new WaitForSeconds(60);
                OneMinutePassed();
            }
            Debug.Log("Tutorialhandler: Main Routine: One minute passed.");
            gameManager.PlayerUI.CloseConditionalNotification();
            #endregion

            #region A Planet Arrives
            yield return new WaitForSeconds(5);
            FiveSecondsPassed();
            gameManager.PlayerUI.SendTimedNotification("We're approaching a planet!");
            gameManager.PlanetRotation.StartTutorialRotationCoroutine();
            gameManager.PlayerUI.SendTimedNotification("Go upstairs to trade!");
            UnlockMainDoor();
            // Activate indications to go upstairs
            gameManager.DebtUI.DeactivateDebtUI();
            yield return new WaitForSeconds(5);
            #endregion

            #region Make 600 Credits
            gameManager.PlayerUI.SendConditionalNotification("Make 600 credits!");
            while (conditions["Made600Credits"] == false)
            {
                if (GameManager.Instance.PlayerCredits >= 600) Made600Credits();
                yield return new WaitForSeconds(checkInterval);
            }
            Debug.Log("Tutorialhandler: Main Routine: Made 600 credits.");
            gameManager.PlayerUI.CloseConditionalNotification();
            #endregion

            #region Make Debt Payment
            gameManager.DebtUI.ActivateDebtUI();
            gameManager.PlayerUI.SendConditionalNotification("Make a debt payment!");
            while (conditions["MadeDebtPayment"] == false)
            {
                yield return new WaitForSeconds(checkInterval);
            }
            Debug.Log("Tutorialhandler: Main Routine: Made debt payment.");
            gameManager.PlayerUI.CloseConditionalNotification();
            #endregion

            gameManager.PlayerUI.SendTimedNotification("Thank you for your payment.");

            yield return new WaitForSeconds(5);

            gameManager.PlayerUI.SendTimedNotification("Now get back to work!");

            UnSetUpTutorial();
            gameManager.StartGameFromTutorial(); // Sets InTutorial to false
        }
    }

    private void UnSetUpTutorial()
    {
        machineSlots[3].SetActive(true);
        gameManager.PlanetRotation.StartReturnToStartingRotationCoroutine();
    }


    private void UnlockMainDoor()
    {
        doorColliders[0].enabled = true;
    }

    private void ToggleAllDoors(bool unlocked)
    {
        foreach (Collider doorCollider in doorColliders)
        {
            doorCollider.enabled = unlocked;
        }
    }

    public void PlacedResourceInMachine(ResourceTypes resourceType)
    {
        switch (resourceType)
        {
            case ResourceTypes.CROP:
                cropsPlacedInMachine++;
                if (cropsPlacedInMachine >= 1) OneCropInPartsMachine();
                break;
            case ResourceTypes.PART:
                partsPlacedInMachine++;
                if (partsPlacedInMachine >= 2) TwoPartsInNitrogenMachine();
                break;
            case ResourceTypes.NITROGEN:
                nitrogenPlacedInMachine++;
                if (nitrogenPlacedInMachine >= 6) SixNitrogenInCropsMachine();
                break;
            default:
                break;
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

    public void OneCropInPartsMachine()
    {
        conditions["OneCropInPartsMachine"] = true;
    }

    public void TwoPartsInNitrogenMachine()
    {
        conditions["TwoPartsInNitrogenMachine"] = true;
    }

    public void SixNitrogenInCropsMachine()
    {
        conditions["SixNitrogenInCropsMachine"] = true;
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
