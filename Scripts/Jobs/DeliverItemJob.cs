using System.Threading;
using System.Threading.Tasks;
using Godot;

public class DeliverItemJob : IJob
{
    readonly ItemAmount _item;
    readonly ulong _amount;
    readonly IInventoryAdd _inventory;

    public DeliverItemJob(ItemAmount item, ulong amount, IInventoryAdd inventory)
    {
        _item = item;
        _amount = amount;
        _inventory = inventory;

        _item.Reserve(_amount);
    }

    public async Task Execute(PhysicsBody2D executor, CancellationToken ct)
    {
        var mover = executor.GetNode<Mover>("Mover");

        try
        {
            ct.ThrowIfCancellationRequested();

            await mover.MoveTo(_item.GlobalPosition, ct);
        }
        catch (TaskCanceledException)
        {
            _item.Unreserve(_amount);
            throw;
        }

        var backpack = executor.GetNode<Backpack>("Backpack");
        _item.Remove(_amount);
        backpack.Add(_item.Def, _amount);

        try
        {
            await mover.MoveTo(_inventory.GlobalPosition, ct);

            backpack.Remove(_item.Def, _amount);
            _inventory.Add(_item.Def, _amount);
        }
        catch (TaskCanceledException)
        {
            backpack.Dump();
            throw;
        }
    }
}
