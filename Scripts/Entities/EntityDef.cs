using Godot;

public abstract partial class EntityDef : Resource
{
    [Export]
    public string ScenePath;

    public PackedScene GetPackedScene() => ResourceLoader.Load<PackedScene>(ScenePath);
}
