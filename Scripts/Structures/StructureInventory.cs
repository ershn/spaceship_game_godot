using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Godot;

public partial class StructureInventory : Node, IInventoryAdd, IInventoryRemove
{
    class ItemSlot
    {
        public event Action<bool> OnFull;

        readonly ulong _maxAmount;
        public ulong MaxAmount => _maxAmount;

        ulong _currentAmount;
        public ulong CurrentAmount => _currentAmount;

        public ulong? RefillThreshold { get; }

        public ulong RequestedAmount;

        public ItemSlot(ulong maxAmount, ulong? refillThreshold = null)
        {
            _maxAmount = maxAmount;
            RefillThreshold = refillThreshold;
        }

        public bool Full => _currentAmount == _maxAmount;

        public void IncreaseAmount(ulong amount)
        {
            Debug.Assert(amount > 0);
            Debug.Assert(_currentAmount + amount <= _maxAmount);

            _currentAmount += amount;
            if (_currentAmount == _maxAmount)
                OnFull?.Invoke(true);
        }

        public void DecreaseAmount(ulong amount)
        {
            Debug.Assert(amount > 0);
            Debug.Assert(amount <= _currentAmount);

            _currentAmount -= amount;
            if (_currentAmount + amount == _maxAmount)
                OnFull?.Invoke(false);
        }

        public void ZeroAmount()
        {
            var previousAmount = _currentAmount;
            _currentAmount = 0;
            if (previousAmount == _maxAmount)
                OnFull?.Invoke(false);
        }
    }

    [Signal]
    public delegate void OnFullEventHandler(bool full);

    ItemCreator _itemCreator;
    ItemAllotter _itemAllotter;

    [Export]
    GridPosition _gridPosition;

    readonly Dictionary<ItemDef, ItemSlot> _slots = new();
    uint _fullSlotCount = 0;

    readonly CancellationTokenSource _itemRequestCanceller = new();

    public override void _Ready()
    {
        _itemCreator = Owner.GetNode<ItemCreator>("../%ItemCreator");
        _itemAllotter = Owner.GetNode<ItemAllotter>("../%ItemAllotter");

        foreach (var (itemDef, slot) in _slots)
            InitSlot(itemDef, slot);
    }

    public override void _ExitTree()
    {
        _itemRequestCanceller.Dispose();
    }

    public bool Setup => _slots.Count > 0;

    public bool Full => _fullSlotCount == _slots.Count;

    public void AddSlot(ItemDef itemDef, ulong maxAmount) => AddSlot(itemDef, maxAmount, null);

    public void AddSlot(ItemDef itemDef, ulong maxAmount, ulong refillThreshold) =>
        AddSlot(itemDef, maxAmount, (ulong?)refillThreshold);

    void AddSlot(ItemDef itemDef, ulong maxAmount, ulong? refillThreshold)
    {
        Debug.Assert(maxAmount > 0);
        Debug.Assert(refillThreshold == null || refillThreshold < maxAmount);

        var slot = new ItemSlot(maxAmount, refillThreshold);
        _slots[itemDef] = slot;
        InitSlot(itemDef, slot);
    }

    void InitSlot(ItemDef itemDef, ItemSlot slot)
    {
        slot.OnFull += OnSlotFull;
        RequestRefill(itemDef, slot);
        if (slot.Full)
            _fullSlotCount++;
    }

    void OnSlotFull(bool full)
    {
        var previouslyFull = Full;

        if (full)
            _fullSlotCount++;
        else
            _fullSlotCount--;

        if (previouslyFull != Full)
            EmitSignal(SignalName.OnFull, Full);
    }

    public IEnumerable<(ItemDef, ulong)> UnfilledSlots() =>
        _slots
            .Where(kv => kv.Value.CurrentAmount < kv.Value.MaxAmount)
            .Select(kv => (kv.Key, kv.Value.MaxAmount - kv.Value.CurrentAmount));

    public void Add(ItemDef itemDef, ulong amount)
    {
        Debug.Assert(amount > 0);

        var slot = _slots[itemDef];
        Debug.Assert(slot.CurrentAmount + amount <= slot.MaxAmount);

        slot.IncreaseAmount(amount);
    }

    public void Remove(ItemDef itemDef, ulong amount)
    {
        Debug.Assert(amount > 0);

        var slot = _slots[itemDef];
        Debug.Assert(amount <= slot.CurrentAmount);

        slot.DecreaseAmount(amount);
        RequestRefill(itemDef, slot);
    }

    public bool TryRemove(ItemDef itemDef, ulong amount)
    {
        Debug.Assert(amount > 0);

        var slot = _slots[itemDef];
        if (amount <= slot.CurrentAmount)
        {
            slot.DecreaseAmount(amount);
            RequestRefill(itemDef, slot);
            return true;
        }
        else
            return false;
    }

    void RequestRefill(ItemDef itemDef, ItemSlot slot)
    {
        if (slot.RefillThreshold is null)
            return;

        var futureAmount = slot.CurrentAmount + slot.RequestedAmount;
        if (futureAmount > slot.RefillThreshold)
            return;

        var requestAmount = slot.MaxAmount - futureAmount;
        slot.RequestedAmount += requestAmount;

        _itemAllotter.Request(
            itemDef,
            requestAmount,
            this,
            _itemRequestCanceller.Token,
            deliveredAmount => slot.RequestedAmount -= deliveredAmount
        );
    }

    public void Dump()
    {
        _itemRequestCanceller.Cancel();

        var cellPosition = _gridPosition.CellPosition;
        foreach (var (itemDef, slot) in _slots)
        {
            if (slot.CurrentAmount > 0)
            {
                _itemCreator.Create(cellPosition, itemDef, slot.CurrentAmount);
                slot.ZeroAmount();
            }
        }
    }

    public Vector2 GlobalPosition => GetOwner<Node2D>().GlobalPosition;

    public Vector2 Position => GetOwner<Node2D>().Position;
}
