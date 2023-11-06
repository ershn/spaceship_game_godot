using Godot;

[GlobalClass]
public partial class GridPosition : Node, IPosition
{
    INodeGrid _grid;

    TileMap _tileMap;

    public Vector2I CellPosition => _tileMap.LocalToMap(Position);

    public override void _EnterTree()
    {
        _tileMap = Owner.GetNode<TileMap>("../%WorldTileMap");

        AddToGrid();
    }

    public override void _ExitTree()
    {
        RemoveFromGrid();
    }

    void AddToGrid()
    {
        var worldLayerConf = GetNodeOrNull<IWorldLayerGet>("../DefHolder");
        if (worldLayerConf is not null)
        {
            var worldLayer = worldLayerConf.WorldLayer;
            _grid = Owner.GetNode<Grids>("../%Grids").GetLayerGrid(worldLayer);
            _grid.Add(CellPosition, Owner);
        }
    }

    void RemoveFromGrid()
    {
        _grid?.Remove(CellPosition, Owner);
    }

    public Vector2 GlobalPosition => GetOwner<Node2D>().GlobalPosition;

    public Vector2 Position => GetOwner<Node2D>().Position;
}
