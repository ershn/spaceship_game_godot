using Godot;

[GlobalClass]
public partial class FloorConstructor : Node
{
    [Export]
    StructureConstructor _structureConstructor;

    public override void _Ready()
    {
        _ = _structureConstructor.Construct();
    }
}
