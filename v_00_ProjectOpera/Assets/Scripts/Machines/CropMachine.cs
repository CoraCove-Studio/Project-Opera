using UnityEngine;

public class CropMachine : MachineBehavior
{
    public override ResourceTypes MachineType { get; } = ResourceTypes.CROP;

    public override void Upgrade(float reduction)
    {
        Debug.Log(gameObject.name + " upgraded!");
    }
}
