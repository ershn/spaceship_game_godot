using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;

public sealed class EatFoodJob : IJob, IDisposable
{
    readonly (ItemAmount itemAmount, ulong markedAmount)[] _foodItems;
    int nextFoodItemIndex;
    readonly IWork _foodConsumption;

    public EatFoodJob(
        IEnumerable<(ItemAmount itemAmount, ulong markedAmount)> foodItems,
        IWork foodConsumption
    )
    {
        _foodItems = foodItems.ToArray();
        _foodConsumption = foodConsumption;

        foreach (var (itemAmount, markedAmount) in _foodItems)
            itemAmount.Reserve(markedAmount);
    }

    public void Dispose()
    {
        for (; nextFoodItemIndex < _foodItems.Length; nextFoodItemIndex++)
        {
            var (itemAmount, markedAmount) = _foodItems[nextFoodItemIndex];
            itemAmount.Unreserve(markedAmount);
        }
    }

    public async Task Execute(PhysicsBody2D executor, CancellationToken ct)
    {
        var mover = executor.GetNode<Mover>("Mover");
        var backpack = executor.GetNode<Backpack>("Backpack");
        try
        {
            foreach (var (itemAmount, markedAmount) in _foodItems)
            {
                await mover.MoveTo(itemAmount.GlobalPosition, ct);
                itemAmount.Remove(markedAmount);
                backpack.Add(itemAmount.Def, markedAmount);
                nextFoodItemIndex++;
            }

            var worker = executor.GetNode<Worker>("Worker");
            await worker.WorkOn(_foodConsumption, ct);
        }
        catch (TaskCanceledException)
        {
            backpack.Dump();
            throw;
        }
    }
}
