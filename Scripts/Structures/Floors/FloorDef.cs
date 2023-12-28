using Godot;

[Tool, GlobalClass]
public partial class FloorDef : StructureDef
{
    public override Resource NewState() => new FloorState(this);

    public override WorldLayer WorldLayer => WorldLayer.Floor;

    [ExportGroup("Construction restrictions")]
    [Export]
    public FloorCategory Category;

    public override bool IsConstructibleAt(
        EntityGrids entityGrids,
        Vector2I coord,
        bool ignoreExisting = false
    ) => ignoreExisting || !entityGrids.FloorGrid.Has(coord);
}
