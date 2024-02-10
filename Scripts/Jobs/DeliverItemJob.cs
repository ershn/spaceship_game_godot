using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

public sealed class DeliverItemJob : IJob, IDisposable
{
    ItemAmount _item;
    readonly ItemDef _itemDef;
    readonly ulong _amount;
    readonly IInventoryAdd _inventory;

    public DeliverItemJob(ItemAmount item, ulong amount, IInventoryAdd inventory)
    {
        _item = item;
        _itemDef = _item.Def;
        _amount = amount;
        _inventory = inventory;

        _item.Reserve(_amount);
    }

    public void Dispose()
    {
        _item?.Unreserve(_amount);
    }

    public async Task Execute(PhysicsBody2D executor, CancellationToken ct)
    {
        var mover = executor.GetNode<Mover>("Mover");
        await mover.MoveTo(_item.GlobalPosition, ct);

        var backpack = executor.GetNode<Backpack>("Backpack");
        _item.Remove(_amount);
        backpack.Add(_item.Def, _amount);
        _item = null;

        try
        {
            await mover.MoveTo(_inventory.GlobalPosition, ct);
        }
        catch (TaskCanceledException)
        {
            backpack.Dump();
            throw;
        }

        backpack.Remove(_itemDef, _amount);
        _inventory.Add(_itemDef, _amount);
    }

    public override string ToString() =>
        $"[{nameof(DeliverItemJob)}: {_amount} of {_itemDef}"
        + $" from {_item.GlobalPosition} to {_inventory.GlobalPosition}]";
}
