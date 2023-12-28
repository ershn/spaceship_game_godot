using Godot;

[Tool, GlobalClass]
public partial class GridPosition : Node, IPosition
{
    TileMap _tileMap;

    public bool Valid => _tileMap is not null;

    public Vector2I Coord => _tileMap.LocalToMap(Position);

    public override void _EnterTree()
    {
        _tileMap = Owner.GetNodeOrNull<TileMap>("../%WorldTileMap");
    }

    public Vector2 GlobalPosition => GetOwner<Node2D>().GlobalPosition;

    public Vector2 Position => GetOwner<Node2D>().Position;
}
