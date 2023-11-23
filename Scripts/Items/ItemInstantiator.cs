using Godot;

[GlobalClass]
public partial class ItemInstantiator : Node
{
    [Export]
    TileMap _tileMap;

    ulong _instanceNumber = 100;

    public Node2D Instantiate(Vector2I cellPosition, ItemDef itemDef, ulong amount)
    {
        var item = itemDef.GetPackedScene().Instantiate<Node2D>();

        item.Name = itemDef.ResourceName + _instanceNumber++;
        item.Position = _tileMap.MapToLocal(cellPosition);

        item.GetNode<ItemAmount>("ItemAmount").Initialize(amount);

        GetParent().AddChild(item, forceReadableName: true);

        return item;
    }
}
