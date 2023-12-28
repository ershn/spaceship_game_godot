using Godot;

[GlobalClass]
public abstract partial class AmountMode : Resource
{
    public abstract AmountType AmountType { get; }

    public abstract ulong AmountToMass(ulong amount);

    public abstract ulong DefaultAmount { get; }
}
