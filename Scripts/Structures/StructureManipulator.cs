using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class StructureManipulator : Node
{
    [Export]
    EntityGrids _entityGrids;

    [Export]
    StructureInstantiator _structureInstantiator;

    public void Construct(Vector2I cellPosition, StructureDef structureDef)
    {
        if (structureDef.IsConstructibleAt(_entityGrids, cellPosition))
            _structureInstantiator.Instantiate(cellPosition, structureDef);
    }

    public void Deconstruct(Vector2I cellPosition, WorldLayer structureLayers)
    {
        foreach (var structure in StructuresAt(cellPosition, structureLayers))
            _ = structure.GetNode<StructureDeconstructor>("StructureDeconstructor").Deconstruct();
    }

    public void Cancel(Vector2I cellPosition, WorldLayer structureLayers)
    {
        foreach (var structure in StructuresAt(cellPosition, structureLayers))
            structure.GetNode<Canceler>("Canceler").Cancel();
    }

    IEnumerable<Node> StructuresAt(Vector2I cellPosition, WorldLayer structureLayers)
    {
        foreach (var grid in _entityGrids.GetStructureLayerGrids(structureLayers))
        {
            if (grid.TryGet(cellPosition, out var structure))
                yield return structure;
        }
    }
}
