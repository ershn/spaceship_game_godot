using Godot;

[GlobalClass]
public abstract partial class EntityDefHolder : Node
{
    public abstract EntityDef EntityDef { get; }
}
