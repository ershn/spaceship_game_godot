using Godot;

[Tool, GlobalClass]
public partial class FoodItemDef : ItemDef
{
    [Export]
    MassMode _massMode;
    public override AmountMode AmountMode => _massMode;

    [Export]
    public uint GramToCaloriesMultiplier = 1000;

    public ulong MassToCalories(ulong mass) => mass / 1.Gram() * GramToCaloriesMultiplier;

    public ulong CaloriesToMass(ulong calories) => calories / GramToCaloriesMultiplier * 1.Gram();
}
