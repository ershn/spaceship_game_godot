using Godot;

[Tool, GlobalClass]
public partial class TerrainRef : TileRef
{
    [Export]
    public int TerrainSetId;

    [Export]
    public int TerrainId;
}
