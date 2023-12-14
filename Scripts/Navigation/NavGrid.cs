using Godot;

[GlobalClass]
public partial class NavGrid : Node2D
{
    ArrayGrid<NavCell> _grid;

    [Export]
    Vector2I _mapSize;

    [Export]
    Vector2I _cellSize;

    public override void _Ready()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        _grid = new(_mapSize);
        for (var x = -(_mapSize.X / 2); x < _mapSize.X / 2 + _mapSize.X % 2; x++)
        {
            for (var y = -(_mapSize.Y / 2); y < _mapSize.Y / 2 + _mapSize.Y % 2; y++)
            {
                var coord = new Vector2I(x, y);
                var cell = new NavCell(_cellSize) { Position = _cellSize * coord };
                _grid[coord] = cell;
                AddChild(cell);
            }
        }
    }

    public NavCell this[Vector2I coord] => _grid[coord];
}
