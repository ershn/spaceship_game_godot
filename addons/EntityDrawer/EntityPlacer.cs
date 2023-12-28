#if TOOLS
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace EntityDrawer;

public class EntityPlacer
{
    readonly EntityGrids _entityGrids;
    readonly ItemInstantiator _itemInstantiator;
    readonly StructureInstantiator _structureInstantiator;

    public EntityPlacer(TileMap tileMap)
    {
        _entityGrids = tileMap.GetNode<EntityGrids>("%EntityGrids");
        _itemInstantiator = tileMap.GetNode<ItemInstantiator>("%ItemInstantiator");
        _structureInstantiator = tileMap.GetNode<StructureInstantiator>("%StructureInstantiator");
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

    public bool TryPlaceEntity(Vector2I coord, EntityDef entityDef, Resource entityState)
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

    void PlaceEntity(Vector2I coord, Resource entityState)
    {
        switch (entityState)
        {
            case ItemState itemState:
                _itemInstantiator.EditorInstantiate(coord, itemState);
                break;
            case StructureState structureState:
                _structureInstantiator.EditorInstantiate(coord, structureState);
                break;
        }
    }

    void RemoveInconsistentEntities(Vector2I coord)
    {
        // if (!_entityGrids.FurnitureGrid.TryGet(position, out var furniture))
        //     return;

        // var furnitureDef = furniture.GetComponent<FurnitureDefHolder>().FurnitureDef;
        // if (furnitureDef.IsConstructibleAt(entityGrids, position, ignoreExisting: true))
        //     return;

        // Undo.DestroyObjectImmediate(furniture);
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
