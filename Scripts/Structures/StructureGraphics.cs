using Godot;

[GlobalClass]
public partial class StructureGraphics : Node, ITemplate<StructureDef>
{
    public void Template(StructureDef structureDef)
    {
        structureDef.StructureGraphicsDef.Template(GetOwner<Node2D>());
    }

    [Signal]
    public delegate void OnConstructionCompletedEventHandler();

    [Signal]
    public delegate void OnSetupProgressedEventHandler(float progress);

    public void ConstructionCompleted() => EmitSignal(SignalName.OnConstructionCompleted);

    public void SetupProgressed(float progress) =>
        EmitSignal(SignalName.OnSetupProgressed, progress);
}
