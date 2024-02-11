#nullable enable
using System;
using Godot;

namespace LogicGraphs;

[Tool]
public partial class ItemProducer : CompletingNode
{
    [Export(PropertyHint.Range, "0,1")]
    float _probability = 1f;

    [Export]
    ItemDef _itemDef = null!;

    [Export(hintString: AmountHint.ModeOf + nameof(_itemDef))]
    ulong _amount;

    ItemCreator _itemCreator = null!;
    GridPosition _gridPosition = null!;

    bool _completed;

    protected override void _Ready()
    {
        _itemCreator = Entity.GetNode<ItemCreator>("../%ItemCreator");
        _gridPosition = Entity.GetNode<GridPosition>("GridPosition");
    }

    protected override bool _Process()
    {
        if (!_completed && Random.Shared.NextSingle() <= _probability)
        {
            _itemCreator.Create(_gridPosition.Coord, _itemDef, _amount);
            _completed = true;
        }
        return _completed;
    }

    protected override void _Reset()
    {
        _completed = false;
    }
}
