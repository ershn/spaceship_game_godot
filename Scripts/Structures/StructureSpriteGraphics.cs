using Godot;

[GlobalClass]
public partial class StructureSpriteGraphics : Sprite2D
{
    void OnConstructionCompleted() => SelfModulate = Colors.White;
}
