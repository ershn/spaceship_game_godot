using Godot;

[GlobalClass]
public partial class NavCellController : Node
{
    NavGrid _navGrid;

    [Export]
    GridPosition _gridPosition;

    public override void _Ready()
    {
        _navGrid = Owner.GetNode<NavGrid>("../%NavGrid");
    }

    NavCell NavCell => _navGrid[_gridPosition.CellPosition];

    public bool Traversable
    {
        get => NavCell.Traversable;
        set => NavCell.Traversable = value;
    }
}
