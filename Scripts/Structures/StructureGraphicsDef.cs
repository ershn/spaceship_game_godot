using Godot;

[GlobalClass]
public abstract partial class StructureGraphicsDef : Resource
{
    public abstract Texture2D PreviewSprite { get; }
}
