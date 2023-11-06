using Godot;

public class StructureTilePlacer
{
    readonly StructureTileGraphicsDef _def;
    readonly WorldTileMap _tileMap;
    readonly Node2D _structure;

    WorldTileMap.Layer? _currentLayer;
    Vector2I _cellPosition;

    public StructureTilePlacer(StructureTileGraphicsDef def, WorldTileMap tileMap, Node2D structure)
    {
        _def = def;
        _tileMap = tileMap;
        _structure = structure;
    }

    public void ToBlueprintGraphics() => Place(WorldTileMap.Layer.Blueprints);

    public void ToNormalGraphics() => Place(WorldTileMap.Layer.Base);

    void Place(WorldTileMap.Layer layer)
    {
        Remove();

        _cellPosition = _tileMap.LocalToMap(_tileMap.ToLocal(_structure.GlobalPosition));
        _tileMap.SetTile(layer, _cellPosition, _def.TileRef);
        _currentLayer = layer;
    }

    public void Remove()
    {
        if (_currentLayer is WorldTileMap.Layer layer)
        {
            _tileMap.RemoveTile(layer, _cellPosition);
            _currentLayer = null;
        }
    }
}
