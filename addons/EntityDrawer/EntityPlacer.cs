#if TOOLS
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace EntityDrawer;

public class EntityPlacer
{
    readonly EntityInstantiator _entityInstantiator;
    readonly EntityGrids _entityGrids;

    public EntityPlacer(TileMap tileMap)
    {
        _entityInstantiator = tileMap.GetNode<EntityInstantiator>("%EntityInstantiator");
        _entityGrids = tileMap.GetNode<EntityGrids>("%EntityGrids");
    }

    public bool TryPickEntity(Vector2I coord, out EntityDef entityDef)
    {
        var node = _entityGrids.GlobalGrid.Get(coord).FirstOrDefault();
        if (node is null)
        {
            entityDef = null;
            return false;
        }

        entityDef = node.GetNode<EntityDefHolder>("DefHolder").EntityDef;
        return true;
    }

    public bool TryPlaceEntity(Vector2I coord, EntityDef entityDef, EntityState entityState)
    {
        if (!CanPlaceEntity(coord, entityDef, out var removedNodes))
            return false;

        foreach (var node in removedNodes.ToArray())
            node.Free();
        PlaceEntity(coord, entityState);
        RemoveInconsistentEntities(coord);
        return true;
    }

    bool CanPlaceEntity(Vector2I coord, EntityDef entityDef, out IEnumerable<Node> removedNodes) =>
        entityDef switch
        {
            ItemDef itemDef => CanPlaceItem(coord, itemDef, out removedNodes),
            StructureDef structureDef => CanPlaceStructure(coord, structureDef, out removedNodes),
            _ => throw new NotImplementedException()
        };

    bool CanPlaceItem(Vector2I coord, ItemDef itemDef, out IEnumerable<Node> removedNodes)
    {
        if (_entityGrids.ItemGrid.TryGetItem(coord, itemDef, out var item))
            removedNodes = new Node[] { item };
        else
            removedNodes = Array.Empty<Node>();
        return true;
    }

    bool CanPlaceStructure(
        Vector2I coord,
        StructureDef structureDef,
        out IEnumerable<Node> removedNodes
    )
    {
        var grid = _entityGrids.GetStructureLayerGrid(structureDef.WorldLayer);
        if (grid.TryGet(coord, out var structure))
            removedNodes = new Node[] { structure };
        else
            removedNodes = Array.Empty<Node>();
        return structureDef.IsConstructibleAt(_entityGrids, coord, ignoreExisting: true);
    }

    void PlaceEntity(Vector2I coord, EntityState entityState)
    {
        _entityInstantiator.EditorInstantiate(coord, entityState);
    }

    void RemoveInconsistentEntities(Vector2I coord)
    {
        if (!_entityGrids.FurnitureGrid.TryGet(coord, out var furniture))
            return;

        var furnitureDef = furniture.GetNode<FurnitureDefHolder>("DefHolder").FurnitureDef;
        if (!furnitureDef.IsConstructibleAt(_entityGrids, coord, ignoreExisting: true))
            furniture.Free();
    }

    public bool RemoveEntities(Vector2I coord)
    {
        var nodes = _entityGrids.GlobalGrid.Get(coord);
        if (!nodes.Any())
            return false;

        foreach (var node in nodes.ToArray())
            node.Free();
        return true;
    }
}
#endif
