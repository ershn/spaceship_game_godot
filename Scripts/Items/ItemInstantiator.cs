using Godot;

[Tool, GlobalClass]
public partial class ItemInstantiator : Node
{
    [Export]
    TileMap _tileMap;

    ulong _instanceNumber = 1000;

    public Node2D Instantiate(Vector2I coord, ItemDef itemDef, ulong amount)
    {
        var item = itemDef.GetPackedScene().Instantiate<Node2D>();

        item.Name = itemDef.ResourceName + _instanceNumber++;
        item.Position = _tileMap.MapToLocal(coord);

        item.GetNode<ItemAmount>("ItemAmount").Initialize(amount);

        GetParent().AddChild(item, forceReadableName: true);

        return item;
    }

    public Node2D EditorInstantiate(Vector2I coord, ItemState state)
    {
        var packedScene = state.ItemDef.GetPackedScene();
        var item = packedScene.Instantiate<Node2D>(PackedScene.GenEditState.Instance);

        item.Name = state.ItemDef.ResourceName;
        item.Position = _tileMap.MapToLocal(coord);

        ItemAmount.EditorInitialize(item.GetNode("ItemAmount"), state.Amount);

        GetParent().AddChild(item, forceReadableName: true);
        item.Owner = GetTree().EditedSceneRoot;

        GetParent().SetEditableInstance(item, true);
        item.SetDisplayFolded(true);

        return item;
    }
}
