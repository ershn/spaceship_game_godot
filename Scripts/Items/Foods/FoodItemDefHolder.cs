using Godot;

[GlobalClass]
public partial class FoodItemDefHolder : ItemDefHolder
{
    public FoodItemDef FoodItemDef => (FoodItemDef)ItemDef;
}
