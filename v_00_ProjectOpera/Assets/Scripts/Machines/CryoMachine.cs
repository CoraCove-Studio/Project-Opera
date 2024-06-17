using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryoMachine : MachineBehavior
{
    public override ResourceTypes MachineType { get; } = ResourceTypes.NITROGEN;
    public override void UpgradeMachineEfficiency(int increase)
    {
        if (machineEfficiencyLevel < 4 && GameManager.Instance.PlayerCredits >= 50)
        {

            machineEfficiency += increase;
            machineEfficiencyLevel++;
            machineUI.UpdateEfficiencyLevelText(machineEfficiencyLevel);
            GameManager.Instance.TakeCreditsFromPlayer(50);
            Debug.Log(gameObject.name + "Upgraded to " + machineEfficiencyLevel);
        }
    }

    public override void UpgradeOutputInterval(int reduction)
    {
        if (outputIntervalLevel < 4 && GameManager.Instance.PlayerCredits >= 50)
        {
            outputInterval -= reduction;
            outputIntervalLevel++;
            machineUI.SetSliderMaxValue(outputInterval);
            machineUI.UpdateOutputIntervalLevelText(outputIntervalLevel);
            GameManager.Instance.TakeCreditsFromPlayer(50);
            Debug.Log(gameObject.name + "Upgraded to " + outputIntervalLevel);
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
}
