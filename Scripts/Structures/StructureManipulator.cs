using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class StructureManipulator : Node
{
    [Export]
    EntityGrids _entityGrids;

    [Export]
    EntityInstantiator _entityInstantiator;

    public void Construct(Vector2I coord, StructureDef structureDef)
    {
        if (structureDef.IsConstructibleAt(_entityGrids, coord))
            _entityInstantiator.Instantiate(coord, structureDef.NewState());
    }

    public void Deconstruct(Vector2I coord, WorldLayer structureLayers)
    {
        foreach (var structure in StructuresAt(coord, structureLayers))
            _ = structure.GetNode<StructureDeconstructor>("StructureDeconstructor").Deconstruct();
    }

    public void Cancel(Vector2I coord, WorldLayer structureLayers)
    {
        foreach (var structure in StructuresAt(coord, structureLayers))
            structure.GetNode<Canceler>("Canceler").Cancel();
    }

    IEnumerable<Node> StructuresAt(Vector2I coord, WorldLayer structureLayers)
    {
        foreach (var grid in _entityGrids.GetStructureLayerGrids(structureLayers))
        {
            if (grid.TryGet(coord, out var structure))
                yield return structure;
        }
    }
}
