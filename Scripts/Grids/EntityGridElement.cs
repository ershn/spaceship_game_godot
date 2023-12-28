using Godot;

[Tool, GlobalClass]
public partial class EntityGridElement : Node, ISerializationListener
{
    EntityGrids _entityGrids;

    [Export]
    EntityDefHolder _defHolder;

    [Export]
    GridPosition _gridPosition;

    Vector2I _coord;
    INodeGrid _layerGrid;

    bool Valid => _gridPosition.Valid && _entityGrids is not null;

    public override void _EnterTree()
    {
        _entityGrids = Owner.GetNodeOrNull<EntityGrids>("../%EntityGrids");

        TryAddToGrid();
    }

    public override void _ExitTree()
    {
        TryRemoveFromGrid();
    }

    void TryAddToGrid()
    {
        if (!Valid)
            return;

        var worldLayerConf = (IWorldLayerGet)_defHolder;
        _coord = _gridPosition.Coord;
        _layerGrid = _entityGrids.GetLayerGrid(worldLayerConf.WorldLayer);
        _layerGrid.Add(_coord, Owner);
    }

    void TryRemoveFromGrid()
    {
        if (!Valid)
            return;

        _layerGrid.Remove(_coord, Owner);
    }

    public void OnAfterDeserialize() => Callable.From(TryAddToGrid).CallDeferred();

    public void OnBeforeSerialize() => TryRemoveFromGrid();
}
