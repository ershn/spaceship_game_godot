using Godot;

[Tool, GlobalClass]
public partial class StructureSpriteGraphicsDef : StructureGraphicsDef
{
    public override Texture2D PreviewSprite => Sprite;

    [Export]
    public Texture2D Sprite;

    [Export]
    public Color BlueprintColor;
}
