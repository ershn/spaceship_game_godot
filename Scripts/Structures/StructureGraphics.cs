using Godot;

[GlobalClass]
public partial class StructureGraphics : Node
{
    [Signal]
    public delegate void OnConstructionCompletedEventHandler();

    [Signal]
    public delegate void OnSetupProgressedEventHandler(float progress);

    public void ConstructionCompleted() => EmitSignal(SignalName.OnConstructionCompleted);

    public void SetupProgressed(float progress) =>
        EmitSignal(SignalName.OnSetupProgressed, progress);
}
