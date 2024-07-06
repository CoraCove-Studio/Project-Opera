using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class MachineBehavior : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int inputInventory;
    [SerializeField] private int machineEfficiency = 4;
    [SerializeField] private int outputInterval = 8;
    [SerializeField] private int machineEfficiencyLevel = 1;
    [SerializeField] private int maximumInventory = 20;
    [SerializeField] private int machineDurability = 100;
    [SerializeField] private int maximumMachineDurability = 100;
    [SerializeField] private int upgradeCost = 35;
    [SerializeField] private bool isBroken = false;
    [SerializeField] private bool isEmpty = true;

    [SerializeField] private ObjectPooler objPooler;
    [SerializeField] private Transform outputPos;
    [SerializeField] private MachineUI machineUI;
    [SerializeField] private GameObject brokenEffect;
    [SerializeField] private Animator animatorController;

    private List<string> animatorParameters;

    private readonly Dictionary<ResourceTypes, ResourceTypes> resourceTypeRelationships = new()
    {
        { ResourceTypes.CROP, ResourceTypes.NITROGEN },
        { ResourceTypes.PART, ResourceTypes.CROP },
        { ResourceTypes.NITROGEN, ResourceTypes.PART }
    };

    public abstract ResourceTypes MachineType { get; }
    private Coroutine productionCoroutine;

    [Header("Audio")]
    [SerializeField] private AudioSource loopAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] private AudioClip machineProductionLoop;
    [SerializeField] private AudioClip brokenLoop;
    [SerializeField] private AudioClip machineInstantiationNoise;
    [SerializeField] private AudioClip glitchTransitionNoise;
    [SerializeField] private AudioClip productInstantiationNoise;
    [SerializeField] private AudioClip repairNoise;

    [SerializeField] private List<AudioClip> upgradeNoises;

    private void OnEnable()
    {
        objPooler = GameObject.Find("ObjectPooler").GetComponent<ObjectPooler>();
        if (animatorController != null) animatorParameters = GetAllAnimatorParameters(animatorController);
        SetUpMachineUI();
        SetUpAudio();

        productionCoroutine = StartCoroutine(Production());
    }

    private void OnDisable()
    {
        if (productionCoroutine != null)
        {
            StopCoroutine(productionCoroutine);
        }
    }

    #region Core Methods
    private IEnumerator Production()
    {
        GameObject product;

        while (true)
        {
            if (isEmpty == false & isBroken == false)
            {
                //Debug.Log("Starting Production loop.");
                loopAudioSource.Play();
                if (animatorController != null)
                {
                    animatorController.SetBool(animatorParameters[1], true);
                    SetNewAnimationSpeed(outputInterval);
                }
                // Don't reorder StartBarAnimation, inventory decrementation and WaitForSeconds()
                machineUI.StartBarAnimation(outputInterval);
                inputInventory--;
                machineDurability -= 10;
                machineUI.UpdateDurabilityBar(machineDurability);
                yield return new WaitForSeconds(outputInterval);
                for (int i = 0; i < machineEfficiency; i++)
                {
                    machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
                    product = objPooler.ReturnProduct(MachineType);
                    ConfigureProduct(product);
                    GameManager.Instance.UpdateItemsProduced();
                }
                CheckIfEmpty();
                CheckIfBroken();
                if (animatorController != null && (isEmpty || isBroken)) animatorController.SetBool(animatorParameters[1], false);
            }
            else // must be empty or broken
            {
                //Debug.Log("Machine must be empty or broken.");
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public void AddInput()
    {
        if (GameManager.Instance.CheckPlayerResourceValue(1, resourceTypeRelationships[MachineType]) && inputInventory < maximumInventory)
        {
            GameManager.Instance.TakeResourceFromPlayer(1, resourceTypeRelationships[MachineType]);
            if (inputInventory == 0) machineUI.StartBarAnimation(outputInterval); // Touch this with care
            inputInventory += 1;
            inputInventory = Mathf.Clamp(inputInventory, 0, maximumInventory);
            machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
            if (isEmpty)
            {
                isEmpty = false;
                // turn off empty display
            }
        }
        else
        {
            Debug.Log($"MachineBehavior: AddInput: Couldn't add {resourceTypeRelationships[MachineType]}. Machine full or player didn't have enough.");
        }

    }
    private void RepairMachine()
    {
        if (machineDurability < maximumMachineDurability)
        {
            //Debug.Log("Working machine repaired.");
            machineDurability = maximumMachineDurability;
            machineUI.UpdateDurabilityBar(machineDurability);
            sfxAudioSource.PlayOneShot(repairNoise);
        }
        if (isBroken)
        {
            //Debug.Log("Broken machine repaired.");
            loopAudioSource.Stop();
            brokenEffect.SetActive(true);
            loopAudioSource.clip = machineProductionLoop;
            brokenEffect.SetActive(false);
            isBroken = false;
        }
    }

    private void UpgradeMachineEfficiency(int change)
    {
        if (machineEfficiencyLevel < 6 && GameManager.Instance.PlayerCredits >= upgradeCost)
        {
            sfxAudioSource.PlayOneShot(upgradeNoises[machineEfficiencyLevel - 1]);
            machineEfficiency += change;
            outputInterval -= change;
            machineEfficiencyLevel++;
            machineUI.SetSliderMaxValue(outputInterval);
            machineUI.UpdateEfficiencyLevelText(machineEfficiencyLevel);
            GameManager.Instance.TakeCreditsFromPlayer(upgradeCost);
            Debug.Log(gameObject.name + "Upgraded to " + machineEfficiencyLevel);
        }
        else
        {
            GameManager.Instance.audioManager.PlayErrorNoise();
        }
    }

    #endregion

    #region Utility Methods
    private void SetUpAudio()
    {
        if (loopAudioSource != null)
        {
            loopAudioSource = GetComponent<AudioSource>();
            loopAudioSource.clip = machineProductionLoop;
            loopAudioSource.loop = true;

        }
        else
        {
            Debug.Log($"{gameObject.name}: MachineBehavior: loopAudioSource not found!");
        }

        if (sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(machineInstantiationNoise);

        }
        else
        {
            Debug.Log($"{gameObject.name}: MachineBehavior: sfxAudioSource not found!");
        }
    }

    private void SetUpMachineUI()
    {
        machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
        machineUI.SetSliderMaxValue(outputInterval);
    }
    private List<string> GetAllAnimatorParameters(Animator animator)
    {
        List<string> resultingList = new();
        var numParams = animator.parameterCount;
        for (int i = 0; i < numParams; i++)
        {
            resultingList.Add(animator.GetParameter(i).name);
        }
        return resultingList;
    }
    private void CheckIfEmpty()
    {
        if (inputInventory == 0)
        {
            isEmpty = true;
            //Debug.Log("Machine empty!");
            loopAudioSource.Stop();
        }
    }

    private void CheckIfBroken()
    {
        if (machineDurability <= 0)
        {
            isBroken = true;
            //Debug.Log("Machine broken!");
            sfxAudioSource.PlayOneShot(glitchTransitionNoise);
            brokenEffect.SetActive(true);
            SwitchToBrokenNoiseLoop();
            GameManager.Instance.UpdateMachinesBroken();
        }
    }

    private void SwitchToBrokenNoiseLoop()
    {
        loopAudioSource.Stop();
        loopAudioSource.clip = brokenLoop;
        loopAudioSource.Play();
    }


    private void ConfigureProduct(GameObject product)
    {
        product.transform.position = outputPos.position;
        product.SetActive(true);
        sfxAudioSource.PlayOneShot(productInstantiationNoise);
    }
    public void DisplayInput()
    {
        GameManager.Instance.DisplayTooltip(resourceTypeRelationships[MachineType], -1);
    }

    public void DisplayPrice()
    {
        GameManager.Instance.DisplayTooltip(-upgradeCost);
    }

    protected void OnClickUpgradeMachineEfficiencyButton(int change)
    {
        UpgradeMachineEfficiency(change);
    }

    protected void OnClickRepairMachine()
    {
        RepairMachine();
    }

    private void SetNewAnimationSpeed(int outputInterval)
    {
        switch (outputInterval)
        {
            case 7:
                animatorController.SetFloat(animatorParameters[0], 1.143f);
                break;
            case 6:
                animatorController.SetFloat(animatorParameters[0], 1.333f);
                break;
            case 5:
                animatorController.SetFloat(animatorParameters[0], 1.6f);
                break;
            case 4:
                animatorController.SetFloat(animatorParameters[0], 2f);
                break;
            case 3:
                animatorController.SetFloat(animatorParameters[0], 2.666f);
                break;
            default:
                animatorController.SetFloat(animatorParameters[0], 1f);
                break;
        }
    }

    #endregion
}
