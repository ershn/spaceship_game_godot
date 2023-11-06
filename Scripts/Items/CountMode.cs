using Godot;

[GlobalClass]
public partial class CountMode : AmountMode
{
    // TODO: display as mass
    [Export]
    ulong _unitMass;

    public override AmountType AmountType => AmountType.Count;

    public override ulong AmountToMass(ulong count) => _unitMass * count;
}
