using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrinterMachine : MachineBehavior
{
    public override ResourceTypes MachineType { get; } = ResourceTypes.PART;
    [SerializeField] private Animator armAnimation;

    public override void RepairMachine()
    {
        if(machineDurability < maximumMachineDurability)
        {
            machineDurability = maximumMachineDurability;
            machineUI.UpdateDurabilityBar(machineDurability);
        }
    }
    public override void UpgradeMachineEfficiency(int increase)
    {
        if(machineEfficiencyLevel < 4 && GameManager.Instance.PlayerCredits >= upgradeCost)
        {
            
            machineEfficiency += increase;
            machineEfficiencyLevel++;
            machineUI.UpdateEfficiencyLevelText(machineEfficiencyLevel);            
            GameManager.Instance.TakeCreditsFromPlayer(upgradeCost);
            Debug.Log(gameObject.name + "Upgraded to " + machineEfficiencyLevel);
        }
    }

    public override void UpgradeOutputInterval(int reduction)
    {
        if(outputIntervalLevel < 4 && GameManager.Instance.PlayerCredits >= upgradeCost)
        {
            outputInterval -= reduction;
            outputIntervalLevel++;
            machineUI.SetSliderMaxValue(outputInterval);
            machineUI.UpdateOutputIntervalLevelText(outputIntervalLevel);
            GameManager.Instance.TakeCreditsFromPlayer(upgradeCost);
            Debug.Log(gameObject.name + "Upgraded to " + outputIntervalLevel);
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
                armAnimation.SetBool("IsPrinting", true);
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
                armAnimation.SetBool("IsPrinting", false);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public void OnClickUpgradeMachineEfficiencyButton(int amount)
    {
        UpgradeMachineEfficiency(amount);
    }

    public void OnClickUpgradeOutputIntervalButton(int amount)
    {
        UpgradeOutputInterval(amount);
    }

    public void OnClickRepairMachine()
    {
        RepairMachine();
    }
}
