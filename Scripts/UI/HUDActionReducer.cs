using System;
using Godot;

[GlobalClass]
public partial class HUDActionReducer : Node
{
    // [SerializeField]
    // WorldCamera _worldCamera;

    [Export]
    Node2D _initialWorld;

    TileMap _worldTileMap;
    StructureManipulator _worldStructureManipulator;

    Action<Vector2> _onWorldClick;

    public override void _Ready()
    {
        SelectWorld(_initialWorld);
    }

    #region worlds

    void SelectWorld(Node2D world)
    {
        _worldTileMap = world.GetNode<TileMap>("%WorldTileMap");
        _worldStructureManipulator = world.GetNode<StructureManipulator>("%StructureManipulator");

        // _worldCamera.SelectWorld(world);
    }

    void WorldClick(Vector2 position)
    {
        _onWorldClick?.Invoke(position);
    }

    #endregion
    #region blueprints

    void SelectBlueprint(StructureDef structureDef)
    {
        _onWorldClick = position => PlaceBlueprint(position, structureDef);
    }

    void PlaceBlueprint(Vector2 position, StructureDef structureDef)
    {
        var coord = _worldTileMap.LocalToMap(position);
        _worldStructureManipulator.Construct(coord, structureDef);
    }

    #endregion
    #region tasks

    readonly WorldLayer _structureLayers = WorldLayer.Floor;

    void SelectCancelTask()
    {
        _onWorldClick = CancelTask;
    }

    void CancelTask(Vector2 position)
    {
        var coord = _worldTileMap.LocalToMap(position);
        _worldStructureManipulator.Cancel(coord, _structureLayers);
    }

    void SelectDeconstructTask()
    {
        _onWorldClick = DeconstructTask;
    }

    void DeconstructTask(Vector2 position)
    {
        var coord = _worldTileMap.LocalToMap(position);
        _worldStructureManipulator.Deconstruct(coord, _structureLayers);
    }

    #endregion
}
