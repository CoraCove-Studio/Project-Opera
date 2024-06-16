using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryoMachine : MachineBehavior
{
    public override ResourceTypes MachineType { get; } = ResourceTypes.NITROGEN;

    public override void Upgrade(float reduction)
    {
        throw new System.NotImplementedException();
    }
}
