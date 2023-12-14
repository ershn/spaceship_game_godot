using Godot;

[GlobalClass]
public partial class EntityGridElement : Node
{
    EntityGrids _entityGrids;

    [Export]
    EntityDefHolder _defHolder;

    [Export]
    GridPosition _gridPosition;

    Vector2I _cellPosition;
    INodeGrid _layerGrid;

    public override void _Ready()
    {
        _entityGrids = Owner.GetNode<EntityGrids>("../%EntityGrids");

        AddToGrid();
    }

    public override void _ExitTree()
    {
        RemoveFromGrid();
    }

    void AddToGrid()
    {
        var worldLayerConf = (IWorldLayerGet)_defHolder;
        _cellPosition = _gridPosition.CellPosition;
        _layerGrid = _entityGrids.GetLayerGrid(worldLayerConf.WorldLayer);
        _layerGrid.Add(_cellPosition, Owner);
    }

    void RemoveFromGrid()
    {
        _layerGrid.Remove(_cellPosition, Owner);
    }
}
