#nullable enable
using Godot;

[GlobalClass]
public partial class StructureComponentInventory : StructureInventory
{
    public override void _Ready()
    {
        base._Ready();

        if (!Setup)
        {
            var structureDef = GetNode<StructureDefHolder>("../DefHolder").StructureDef;
            foreach (var component in structureDef.ComponentAmounts)
                AddSlot(component.ItemDef, component.Amount);
        }
    }
}
