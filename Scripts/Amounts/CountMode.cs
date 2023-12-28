using Godot;

[Tool, GlobalClass]
public partial class CountMode : AmountMode
{
    public override AmountType AmountType => AmountType.Count;

    [Export(hintString: AmountHint.Mass)]
    ulong _unitMass;

    public override ulong AmountToMass(ulong count) => _unitMass * count;

    [Export(hintString: AmountHint.Count)]
    ulong _defaultCount;

    public override ulong DefaultAmount => _defaultCount;
}
