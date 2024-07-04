using System.Collections;
using UnityEngine;

public class CropMachine : MachineBehavior
{
    public override ResourceTypes MachineType { get; } = ResourceTypes.CROP;
    [SerializeField] private Animator cropAnimator;

    public override void RepairMachine()
    {
        if (machineDurability < maximumMachineDurability)
        {
            machineDurability = maximumMachineDurability;
            machineUI.UpdateDurabilityBar(machineDurability);
            // Play a repair noise, drill sound?
        }
    }

    public override void UpgradeMachineEfficiency(int change)
    {
        if (machineEfficiencyLevel < 6 && GameManager.Instance.PlayerCredits >= upgradeCost)
        {
            machineEfficiency += change;
            outputInterval -= change;
            machineEfficiencyLevel++;
            machineUI.SetSliderMaxValue(outputInterval);
            machineUI.UpdateEfficiencyLevelText(machineEfficiencyLevel);
            GameManager.Instance.TakeCreditsFromPlayer(upgradeCost);
            Debug.Log(gameObject.name + "Upgraded to " + machineEfficiencyLevel);
            SetNewAnimationSpeed(outputInterval);
            // Play a successful noise: level up sound?
        }
    }

    private void SetNewAnimationSpeed(int outputInterval)
    {
        switch (outputInterval)
        {
            case 7:
                cropAnimator.SetFloat("speedMultiplier", 1.143f);
                break;
            case 6:
                cropAnimator.SetFloat("speedMultiplier", 1.333f);
                break;
            case 5:
                cropAnimator.SetFloat("speedMultiplier", 1.6f);
                break;
            case 4:
                cropAnimator.SetFloat("speedMultiplier", 2f);
                break;
            case 3:
                cropAnimator.SetFloat("speedMultiplier", 2.666f);
                break;
            default:
                cropAnimator.SetFloat("speedMultiplier", 8f);
                break;
        }
    }

    protected override IEnumerator Production()
    {
        GameObject product;

        while (true)
        {
            if (inputInventory > 0 & machineDurability > 0)
            {
                audioSource.Play();
                cropAnimator.SetBool("isGrowing", true);
                // Don't reorder StartBarAnimation, inventory decrementation and WaitForSeconds()
                machineUI.StartBarAnimation(outputInterval);
                inputInventory--;
                machineDurability -= 10;
                yield return new WaitForSeconds(outputInterval);
                for (int i = 0; i < machineEfficiency; i++)
                {
                    machineUI.UpdateInventoryLabel(inputInventory, maximumInventory);
                    product = objPooler.ReturnProduct(MachineType);
                    ConfigureProduct(product);
                }
                machineUI.UpdateDurabilityBar(machineDurability);
            }
            else
            {
                audioSource.Stop();
                cropAnimator.SetBool("isGrowing", false);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public void OnClickUpgradeMachineEfficiencyButton(int change)
    {
        UpgradeMachineEfficiency(change);
    }

    public void OnClickRepairMachine()
    {
        RepairMachine();
    }
}
