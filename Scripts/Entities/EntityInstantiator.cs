using Godot;

[Tool, GlobalClass]
public partial class EntityInstantiator : Node
{
    [Export]
    TileMap _tileMap;

    ulong _nextInstanceId = 1000;

    public Node2D Instantiate(Vector2I coord, EntityState state)
    {
        var entity = state.EntityDef.GetPackedScene().Instantiate<Node2D>();

        entity.Name = state.EntityDef.ResourceName + _nextInstanceId++;
        entity.Position = _tileMap.MapToLocal(coord);

        state.Initialize(entity);

        //? Should the call be deferred ?
        GetParent().AddChild(entity, forceReadableName: true);

        return entity;
    }

    public Node2D EditorInstantiate(Vector2I coord, EntityState state)
    {
        var packedScene = state.EntityDef.GetPackedScene();
        var entity = packedScene.Instantiate<Node2D>(PackedScene.GenEditState.Instance);

        entity.Name = state.EntityDef.ResourceName;
        entity.Position = _tileMap.MapToLocal(coord);

        state.EditorInitialize(entity);

        var parent = GetParent();
        parent.AddChild(entity, forceReadableName: true);
        entity.Owner = GetTree().EditedSceneRoot;

        parent.SetEditableInstance(entity, true);
        entity.SetDisplayFolded(true);

        return entity;
    }
}
