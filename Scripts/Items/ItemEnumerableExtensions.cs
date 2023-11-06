using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class ItemEnumerableExtensions
{
    public static (ItemAmount itemAmount, ulong markedAmount)[] CumulateAmount(
        this IEnumerable<Node> items,
        ulong totalAmount
    ) =>
        items
            .Select(item => item.GetNode<ItemAmount>("ItemAmount"))
            .Where(itemAmount => itemAmount.Amount > 0)
            .SelectWhile(itemAmount =>
            {
                var markedAmount = Math.Min(totalAmount, itemAmount.Amount);
                totalAmount -= markedAmount;
                return (totalAmount > 0, (itemAmount, markedAmount));
            })
            .ToArray();
}
