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

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        if ((string)property["name"] == nameof(FloorDef))
            property["usage"] = (long)PropertyUsageFlags.ReadOnly;
    }
}
