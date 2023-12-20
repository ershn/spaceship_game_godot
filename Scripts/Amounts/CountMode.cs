using Godot;

[Tool, GlobalClass]
public partial class CountMode : AmountMode
{
    [Export(hintString: AmountHint.Mass)]
    ulong _unitMass;

    public override AmountType AmountType => AmountType.Count;

    public override ulong AmountToMass(ulong count) => _unitMass * count;
}
