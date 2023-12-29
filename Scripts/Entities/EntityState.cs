using Godot;

public abstract partial class EntityState : Resource
{
    public abstract EntityDef EntityDef { get; }

    public abstract void Initialize(Node2D entity);
    public abstract void EditorInitialize(Node2D entity);
}
