using Godot;

[GlobalClass]
public partial class StructureSpriteGraphics : Sprite2D
{
    [Export]
    public bool IsConstructionCompleted { get; private set; }

    [Export]
    public float SetupProgress { get; private set; }

    void OnConstructionCompleted() => IsConstructionCompleted = true;

    void OnSetupProgressed(float progress) => SetupProgress = progress;
}
