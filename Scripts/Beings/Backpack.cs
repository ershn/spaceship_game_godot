using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

[GlobalClass]
public partial class Backpack : Node, IInventoryAdd, IInventoryRemove
{
    ItemCreator _itemCreator;

    [Export]
    GridPosition _gridPosition;

    // TODO: display as mass
    [Export]
    ulong _maxMass = 100.KiloGrams();

    public ulong CurrentMass { get; private set; }

    readonly Dictionary<ItemDef, ulong> _inventory = new();

    public override void _Ready()
    {
        _itemCreator = Owner.GetNode<ItemCreator>("../%ItemCreator");
    }

    public (T, ulong) First<T>()
        where T : ItemDef
    {
        var (itemDef, amount) = _inventory.First();
        return ((T)itemDef, amount);
    }

    public bool TryFirst<T>(out (T, ulong) first)
        where T : ItemDef
    {
        if (_inventory.Any())
        {
            first = First<T>();
            return true;
        }
        else
        {
            first = default;
            return false;
        }
    }

    public void Add(ItemDef itemDef, ulong amount)
    {
        var mass = itemDef.AmountMode.AmountToMass(amount);
        Debug.Assert(CurrentMass + mass <= _maxMass);

        if (_inventory.TryGetValue(itemDef, out var currentAmount))
            _inventory[itemDef] = currentAmount + amount;
        else
            _inventory[itemDef] = amount;

        CurrentMass += mass;
    }

    public void Remove(ItemDef itemDef, ulong amount)
    {
        var currentAmount = _inventory[itemDef];
        Debug.Assert(amount <= currentAmount);

        var updatedAmount = currentAmount - amount;
        if (updatedAmount > 0)
            _inventory[itemDef] = updatedAmount;
        else
            _inventory.Remove(itemDef);

        var mass = itemDef.AmountMode.AmountToMass(amount);
        CurrentMass -= mass;
    }

    public void Dump()
    {
        foreach (var (itemDef, amount) in _inventory)
            _itemCreator.Create(_gridPosition.CellPosition, itemDef, amount);

        CurrentMass = 0;
        _inventory.Clear();
    }

    public Vector2 GlobalPosition => GetOwner<Node2D>().GlobalPosition;

    public Vector2 Position => GetOwner<Node2D>().Position;
}
