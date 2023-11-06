using Godot;

[GlobalClass]
public partial class MassMode : AmountMode
{
    public override AmountType AmountType => AmountType.Mass;

    public override ulong AmountToMass(ulong mass) => mass;
}
