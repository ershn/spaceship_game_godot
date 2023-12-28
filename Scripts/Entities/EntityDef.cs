using Godot;

public abstract partial class EntityDef : Resource
{
    [Export]
    public string ScenePath;

    public PackedScene GetPackedScene() => GD.Load<PackedScene>(ScenePath);

    public abstract Resource NewState();

    public abstract Texture2D PreviewSprite { get; }
}
