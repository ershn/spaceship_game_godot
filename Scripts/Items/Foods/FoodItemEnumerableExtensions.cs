using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class FoodItemEnumerableExtensions
{
    public static (FoodItemCalories itemCalories, ulong markedCalories)[] CumulateCalories(
        this IEnumerable<Node> items,
        ulong totalCalories
    ) =>
        items
            .Select(item => item.GetNode<FoodItemCalories>("FoodItemCalories"))
            .Where(itemCalories => itemCalories.TotalCalories > 0)
            .SelectWhile(itemCalories =>
            {
                var markedCalories = Math.Min(totalCalories, itemCalories.TotalCalories);
                totalCalories -= markedCalories;
                return (totalCalories > 0, (itemCalories, markedCalories));
            })
            .ToArray();

    public static IEnumerable<(ItemAmount itemAmount, ulong markedMass)> CaloriesToMass(
        this IEnumerable<(FoodItemCalories itemCalories, ulong markedCalories)> items
    ) =>
        items.Select(item =>
        {
            var (itemCalories, markedCalories) = item;
            return (
                itemCalories.GetNode<ItemAmount>("../ItemAmount"),
                itemCalories.GetMass(markedCalories)
            );
        });
}
