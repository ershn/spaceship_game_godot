using Godot;

[GlobalClass]
public abstract partial class EntityDef : Resource
{
    [Export]
    public string ScenePath;

    public PackedScene GetPackedScene() => GD.Load<PackedScene>(ScenePath);

    public abstract EntityState NewState();

    public abstract Texture2D PreviewSprite { get; }

    public override string ToString() =>
        ResourceName.Length > 0 ? $"[{ResourceName}]" : base.ToString();
}
