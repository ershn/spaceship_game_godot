using Godot;

[GlobalClass]
public partial class Destructor : Node
{
    // Emitted when the entity is destroyed through cancelation/deconstruction
    [Signal]
    public delegate void OnDestructionEventHandler();

    public void Destroy()
    {
        EmitSignal(SignalName.OnDestruction);
        Owner.QueueFree();
    }
}
