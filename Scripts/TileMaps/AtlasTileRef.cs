using Godot;

[Tool, GlobalClass]
public partial class AtlasTileRef : TileRef
{
    [Export]
    public int AtlasId;

    [Export]
    public Vector2I AtlasCoords;

    [Export]
    public int AlternativeId;
}
