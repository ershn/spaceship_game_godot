using Godot;

[GlobalClass]
public partial class FloorDef : StructureDef
{
    public override WorldLayer WorldLayer => WorldLayer.Floor;

    [ExportGroup("Construction restrictions")]
    [Export]
    public FloorCategory Category;

    public override bool IsConstructibleAt(
        Grids grids,
        Vector2I cellPosition,
        bool ignoreExisting = false
    ) => ignoreExisting || !grids.FloorGrid.Has(cellPosition);
}
