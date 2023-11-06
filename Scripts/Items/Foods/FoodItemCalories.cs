using System.Diagnostics;
using Godot;

[GlobalClass]
public partial class FoodItemCalories : Node
{
    [Export]
    ItemAmount _itemAmount;

    FoodItemDef _foodItemDef;

    public override void _Ready()
    {
        _foodItemDef = GetNode<FoodItemDefHolder>("../DefHolder").FoodItemDef;
    }

    public ulong TotalCalories => _foodItemDef.MassToCalories(_itemAmount.Amount);

    public ulong GetCalories(ulong mass)
    {
        Debug.Assert(mass <= _itemAmount.Amount);

        return _foodItemDef.MassToCalories(mass);
    }

    public ulong GetMass(ulong calories)
    {
        Debug.Assert(calories <= TotalCalories);

        return _foodItemDef.CaloriesToMass(calories);
    }
}
