using Godot;

[GlobalClass]
public partial class Canceler : Node
{
    [Signal]
    public delegate void OnCancelEventHandler();

    public void Cancel()
    {
        EmitSignal(SignalName.OnCancel);
    }
}
