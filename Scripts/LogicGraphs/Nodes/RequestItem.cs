#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class RequestItem : LogicNode
{
    [Export]
    ItemDef _itemDef = null!;

    [Export(hintString: AmountHint.ModeOf + nameof(_itemDef))]
    ulong _maxAmount;

    [Export(hintString: AmountHint.ModeOf + nameof(_itemDef))]
    ulong _refillThreshold;

    public void Execute()
    {
        var inventory = Entity.GetNode<StructureResourceInventory>("StructureResourceInventory");
        inventory.AddSlot(_itemDef, _maxAmount, _refillThreshold);
    }
}
