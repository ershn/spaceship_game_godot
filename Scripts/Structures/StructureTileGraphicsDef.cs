using Godot;

[Tool, GlobalClass]
public partial class StructureTileGraphicsDef : StructureGraphicsDef
{
    public override void Template(Node2D structure)
    {
        var tileGraphics = PackedScene.Instantiate<StructureTileGraphics>();
        Templater.Template(tileGraphics, this);
        structure.AddChild(tileGraphics, forceReadableName: true);
        tileGraphics.Owner = structure;
    }

    [Export]
    public PackedScene PackedScene;

    [Export]
    public TileRef TileRef;
}
