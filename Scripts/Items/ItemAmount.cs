using System;
using System.Diagnostics;
using Godot;

[GlobalClass]
public partial class ItemAmount : Node, ITemplate<ItemDef>, IPosition
{
    static readonly ulong s_defaultMass = 100.KiloGrams();
    static readonly ulong s_defaultCount = 1;

    public void Template(ItemDef itemDef)
    {
        _amount = itemDef.AmountMode switch
        {
            MassMode => s_defaultMass,
            CountMode => s_defaultCount,
            _ => throw new NotImplementedException(),
        };
    }

    [Signal]
    public delegate void OnAmountChangedToZeroEventHandler();

    [Signal]
    public delegate void OnAmountChangedEventHandler(long amount);

    [Export]
    ItemDefHolder _itemDefHolder;

    [Export(hintString: AmountHint.ModeOf + "_itemDefHolder/ItemDef")]
    ulong _amount = 1;
    ulong _reservedAmount = 0;

    public void Initialize(ulong amount)
    {
        Debug.Assert(!IsNodeReady());

        _amount = amount;
    }

    public override void _Ready()
    {
        SignalAmountChange(_amount);
    }

    public ItemDef Def => _itemDefHolder.ItemDef;

    public ulong Amount => _amount - _reservedAmount;

    public void Add(ulong amount)
    {
        _amount += amount;
        SignalAmountChange(_amount);
    }

    public void Reserve(ulong amount)
    {
        Debug.Assert(amount <= _amount - _reservedAmount);

        _reservedAmount += amount;
    }

    public void Unreserve(ulong amount)
    {
        Debug.Assert(amount <= _reservedAmount);

        _reservedAmount -= amount;
    }

    public void Remove(ulong amount)
    {
        Debug.Assert(amount <= _reservedAmount);

        _reservedAmount -= amount;
        _amount -= amount;
        SignalAmountChange(_amount);
    }

    void SignalAmountChange(ulong amount)
    {
        EmitSignal(SignalName.OnAmountChanged, amount);
        if (amount == 0)
            EmitSignal(SignalName.OnAmountChangedToZero);
    }

    public Vector2 GlobalPosition => GetOwner<Node2D>().GlobalPosition;

    public Vector2 Position => GetOwner<Node2D>().Position;
}
