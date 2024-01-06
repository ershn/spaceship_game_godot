using Godot;

[Tool, GlobalClass]
public partial class StructureTileGraphicsDef : StructureGraphicsDef
{
    [Export]
    Texture2D _previewSprite;
    public override Texture2D PreviewSprite => _previewSprite;

    [Export]
    public TileRef TileRef;
}
