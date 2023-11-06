using System;
using Godot;

[GlobalClass]
public partial class ItemCreator : Node
{
    public event Action<ItemAmount> OnItemCreated;

    [Export]
    Grids _grids;

    [Export]
    ItemInstantiator _instantiator;

    ItemGrid ItemGrid => _grids.ItemGrid;

    public void Create(Vector2I cellPosition, ItemDef itemDef, ulong amount)
    {
        ItemAmount itemAmount;

        if (ItemGrid.TryGetItem(cellPosition, itemDef, out var item))
        {
            itemAmount = item.GetNode<ItemAmount>("ItemAmount");
            itemAmount.Add(amount);
        }
        else
        {
            item = _instantiator.Instantiate(cellPosition, itemDef, amount);
            itemAmount = item.GetNode<ItemAmount>("ItemAmount");
        }

        OnItemCreated?.Invoke(itemAmount);
    }
}
