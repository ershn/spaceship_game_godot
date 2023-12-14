using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class EntityGrids : Node
{
    public NodeListGrid GlobalGrid { get; }
    public ItemGrid ItemGrid { get; }
    public NodeGrid FloorGrid { get; }
    public NodeGrid FurnitureGrid { get; }

    public EntityGrids()
    {
        GlobalGrid = new();
        ItemGrid = new(GlobalGrid);
        FloorGrid = new(GlobalGrid);
        FurnitureGrid = new(GlobalGrid);
    }

    public INodeGrid GetLayerGrid(WorldLayer worldLayer) =>
        worldLayer switch
        {
            WorldLayer.Item => ItemGrid,
            WorldLayer.Floor => FloorGrid,
            WorldLayer.Furniture => FurnitureGrid,
            _ => throw new System.NotImplementedException(),
        };

    public NodeGrid GetStructureLayerGrid(WorldLayer structureLayer) =>
        structureLayer switch
        {
            WorldLayer.Floor => FloorGrid,
            WorldLayer.Furniture => FurnitureGrid,
            _ => throw new System.NotImplementedException(),
        };

    public IEnumerable<NodeGrid> GetStructureLayerGrids(WorldLayer structureLayers)
    {
        var indexes = new List<NodeGrid>();
        if (structureLayers.HasFlag(WorldLayer.Floor))
            indexes.Add(FloorGrid);
        if (structureLayers.HasFlag(WorldLayer.Furniture))
            indexes.Add(FurnitureGrid);
        return indexes;
    }
}
