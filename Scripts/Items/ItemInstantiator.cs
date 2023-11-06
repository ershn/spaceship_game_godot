using Godot;

[GlobalClass]
public partial class ItemInstantiator : Node
{
    [Export]
    TileMap _tileMap;

    ulong _instanceNumber = 10;

    public Node2D Instantiate(Vector2I cellPosition, ItemDef itemDef, ulong amount)
    {
        var item = itemDef.PackedScene.Instantiate<Node2D>();

        item.Name = itemDef.ResourceName + _instanceNumber++;
        item.Position = _tileMap.MapToLocal(cellPosition);

        item.GetNode<ItemDefHolder>("DefHolder").ItemDef = itemDef;
        item.GetNode<ItemAmount>("ItemAmount").Initialize(amount);

        Callable.From(() => GetParent().AddChild(item, forceReadableName: true)).CallDeferred();

        return item;
    }
}
