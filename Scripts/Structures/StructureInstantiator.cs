using Godot;

[GlobalClass]
public partial class StructureInstantiator : Node
{
    [Export]
    TileMap _tileMap;

    ulong _instanceNumber = 100;

    public Node2D Instantiate(Vector2I cellPosition, StructureDef structureDef)
    {
        var structure = structureDef.GetPackedScene().Instantiate<Node2D>();

        structure.Name = structureDef.ResourceName + _instanceNumber++;
        structure.Position = _tileMap.MapToLocal(cellPosition);

        Callable
            .From(() => GetParent().AddChild(structure, forceReadableName: true))
            .CallDeferred();

        return structure;
    }
}
