using Godot;

[Tool]
public partial class FloorState : StructureState
{
    public override StructureDef StructureDef => FloorDef;

    [Export]
    public FloorDef FloorDef;

    FloorState() { }

    public FloorState(FloorDef floorDef)
    {
        FloorDef = floorDef;
    }
}
