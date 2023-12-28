using System;
using Godot;

[GlobalClass]
public partial class ItemCreator : Node
{
    public event Action<ItemAmount> OnItemCreated;

    [Export]
    EntityGrids _entityGrids;

    [Export]
    ItemInstantiator _instantiator;

    ItemGrid ItemGrid => _entityGrids.ItemGrid;

    public void Create(Vector2I coord, ItemDef itemDef, ulong amount)
    {
        ItemAmount itemAmount;

        if (ItemGrid.TryGetItem(coord, itemDef, out var item))
        {
            itemAmount = item.GetNode<ItemAmount>("ItemAmount");
            itemAmount.Add(amount);
        }
        else
        {
            item = _instantiator.Instantiate(coord, itemDef, amount);
            itemAmount = item.GetNode<ItemAmount>("ItemAmount");
        }

        OnItemCreated?.Invoke(itemAmount);
    }
}
