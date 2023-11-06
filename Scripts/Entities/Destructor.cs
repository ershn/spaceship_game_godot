using Godot;

[GlobalClass]
public partial class Destructor : Node
{
    [Signal]
    public delegate void OnDestructionEventHandler();

    public void Destroy()
    {
        EmitSignal(SignalName.OnDestruction);
        Owner.QueueFree();
    }
}
