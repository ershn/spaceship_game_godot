using Godot;

[GlobalClass]
public abstract partial class StructureGraphicsDef : Resource
{
    public abstract void Template(Node2D structure);

    public abstract Texture2D PreviewSprite { get; }
}
