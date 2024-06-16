using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterMachine : MachineBehavior
{
    public override ResourceTypes MachineType { get; } = ResourceTypes.PART;

    public override void Upgrade(float reduction)
    {
        throw new System.NotImplementedException();
    }
}
