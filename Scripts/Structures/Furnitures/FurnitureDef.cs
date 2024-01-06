using Godot;

[Tool, GlobalClass]
public partial class FurnitureDef : StructureDef
{
    public override FurnitureState NewState() => new(this);

    public override WorldLayer WorldLayer => WorldLayer.Furniture;

    [ExportGroup("Construction restrictions")]
    [Export]
    public FloorCategory PlaceableFloorCategory;

    public override bool IsConstructibleAt(
        EntityGrids entityGrids,
        Vector2I coord,
        bool ignoreExisting = false
    )
    {
        if (!ignoreExisting && entityGrids.FurnitureGrid.Has(coord))
            return false;
        if (!entityGrids.FloorGrid.TryGet(coord, out var floor))
            return false;
        var floorDef = floor.GetNode<FloorDefHolder>("DefHolder").FloorDef;
        return floorDef.Category == PlaceableFloorCategory;
    }
}
