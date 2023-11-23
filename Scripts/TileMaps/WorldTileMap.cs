using System;
using System.Diagnostics;
using Godot;

[Tool, GlobalClass]
public partial class WorldTileMap : TileMap
{
    public enum Layer
    {
        Base,
        Blueprints
    }

    int[] _layerIndexes;

    public WorldTileMap()
    {
        _layerIndexes = CreateLayerIndexTable();
    }

    int[] CreateLayerIndexTable()
    {
        var enumEntryCount = Enum.GetNames<Layer>().Length;
        var indexTable = new int[enumEntryCount];

        var indexCount = GetLayersCount();
        var assignedIndexCount = 0;
        for (var index = 0; index < indexCount; index++)
        {
            var name = GetLayerName(index);
            if (Enum.TryParse<Layer>(name, ignoreCase: true, out var layer))
            {
                indexTable[(int)layer] = index;
                assignedIndexCount++;
            }
        }
        Debug.Assert(indexTable.Length == assignedIndexCount);

        return indexTable;
    }

    int GetLayerIndex(Layer layer) => _layerIndexes[(int)layer];

    Godot.Collections.Array<Vector2I> _singlePositionArray = new(new Vector2I[1]);

    public void SetTile(Layer layer, Vector2I position, TileRef tile)
    {
        switch (tile)
        {
            case AtlasTileRef tileRef:
                SetCell(
                    GetLayerIndex(layer),
                    position,
                    tileRef.AtlasId,
                    tileRef.AtlasCoords,
                    tileRef.AlternativeId
                );
                break;
            case TerrainRef tileRef:
                _singlePositionArray[0] = position;
                SetCellsTerrainConnect(
                    GetLayerIndex(layer),
                    _singlePositionArray,
                    tileRef.TerrainSetId,
                    tileRef.TerrainId
                );
                break;
        }
    }

    public void RemoveTile(Layer layer, Vector2I position)
    {
        _singlePositionArray[0] = position;
        SetCellsTerrainConnect(GetLayerIndex(layer), _singlePositionArray, 0, -1);
    }
}
