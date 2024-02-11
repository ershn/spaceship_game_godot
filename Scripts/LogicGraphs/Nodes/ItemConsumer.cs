#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class ItemConsumer : CompletingNode
{
    [Export]
    ItemDef _itemDef = null!;

    [Export(hintString: AmountHint.ModeOf + nameof(_itemDef))]
    ulong _amount;

    StructureResourceInventory _inventory = null!;

    bool _completed;

    protected override void _Ready()
    {
        _inventory = Entity.GetNode<StructureResourceInventory>("StructureResourceInventory");
    }

    protected override bool _Process()
    {
        if (!_completed)
            _completed = _inventory.TryRemove(_itemDef, _amount);
        return _completed;
    }

    protected override void _Reset()
    {
        _completed = false;
    }
}
