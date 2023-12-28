using Godot;

[Tool, GlobalClass]
public partial class MassMode : AmountMode
{
    public override AmountType AmountType => AmountType.Mass;

    public override ulong AmountToMass(ulong mass) => mass;

    [Export(hintString: AmountHint.Mass)]
    ulong _defaultMass;

    public override ulong DefaultAmount => _defaultMass;
}
