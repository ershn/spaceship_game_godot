using Godot;

[GlobalClass]
public partial class GridPosition : Node, IPosition
{
    TileMap _tileMap;

    public Vector2I CellPosition => _tileMap.LocalToMap(Position);

    public override void _EnterTree()
    {
        _tileMap = Owner.GetNode<TileMap>("../%WorldTileMap");
    }

    public Vector2 GlobalPosition => GetOwner<Node2D>().GlobalPosition;

    public Vector2 Position => GetOwner<Node2D>().Position;
}
