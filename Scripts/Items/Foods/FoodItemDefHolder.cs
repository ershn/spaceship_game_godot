using Godot;

[Tool, GlobalClass]
public partial class FoodItemDefHolder : ItemDefHolder
{
    public FoodItemDef FoodItemDef => (FoodItemDef)ItemDef;
}
