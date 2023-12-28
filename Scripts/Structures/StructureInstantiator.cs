using Godot;

[Tool, GlobalClass]
public partial class StructureInstantiator : Node
{
    [Export]
    TileMap _tileMap;

    ulong _instanceNumber = 1000;

    public Node2D Instantiate(Vector2I coord, StructureDef structureDef)
    {
        var structure = structureDef.GetPackedScene().Instantiate<Node2D>();

        structure.Name = structureDef.ResourceName + _instanceNumber++;
        structure.Position = _tileMap.MapToLocal(coord);

        Callable
            .From(() => GetParent().AddChild(structure, forceReadableName: true))
            .CallDeferred();

        return structure;
    }

    public Node2D EditorInstantiate(Vector2I coord, StructureState state)
    {
        var packedScene = state.StructureDef.GetPackedScene();
        var structure = packedScene.Instantiate<Node2D>(PackedScene.GenEditState.Instance);

        structure.Name = state.StructureDef.ResourceName;
        structure.Position = _tileMap.MapToLocal(coord);

        GetParent().AddChild(structure, forceReadableName: true);
        structure.Owner = GetTree().EditedSceneRoot;

        GetParent().SetEditableInstance(structure, true);
        structure.SetDisplayFolded(true);

        return structure;
    }
}
