using UnityEngine;

public class CropMachine : MachineBehavior
{
    public override ResourceTypes MachineType { get; } = ResourceTypes.CROP;
    public override void RepairMachine()
    {
        if (machineDurability < maximumMachineDurability)
        {
            machineDurability = maximumMachineDurability;
            machineUI.UpdateDurabilityBar(machineDurability);
        }
    }

    public override void UpgradeMachineEfficiency(int change)
    {
        if (machineEfficiencyLevel < 8 && GameManager.Instance.PlayerCredits >= upgradeCost)
        {
            machineEfficiency += change;
            outputInterval -= change;
            machineEfficiencyLevel++;
            machineUI.SetSliderMaxValue(outputInterval);
            machineUI.UpdateEfficiencyLevelText(machineEfficiencyLevel);
            GameManager.Instance.TakeCreditsFromPlayer(upgradeCost);
            Debug.Log(gameObject.name + "Upgraded to " + machineEfficiencyLevel);
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
