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

    ItemDef _itemDef;

    // TODO: display as mass/count
    [Export]
    ulong _amount = 1;
    ulong _reservedAmount = 0;

    bool _started;

    public override void _Ready()
    {
        _itemDef = GetNode<ItemDefHolder>("../DefHolder").ItemDef;
    }

    // TODO: find a better way to simulate a Start
    public override void _Process(double _delta)
    {
        _started = true;
        SignalAmountChange(_amount);
        SetProcess(false);
    }

    public ItemDef Def => _itemDef;

    public void Initialize(ulong amount)
    {
        Debug.Assert(!_started);

        _amount = amount;
    }

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
