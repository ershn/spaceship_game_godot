using System.Collections.Generic;
using Godot;

[Tool, GlobalClass]
public partial class StructureTileGraphics : Node2D, ITemplate<StructureTileGraphicsDef>
{
    public void Template(StructureTileGraphicsDef def)
    {
        TileGraphicsDef = def;
    }

    StructureGraphics _structureGraphics;

    StructureTileGraphicsDef _tileGraphicsDef;

    [Export]
    StructureTileGraphicsDef TileGraphicsDef
    {
        get => _tileGraphicsDef;
        set
        {
            _tileGraphicsDef = value;
            if (Engine.IsEditorHint())
                EditorUpdate();
        }
    }

    WorldTileMap WorldTileMap => GetNodeOrNull<WorldTileMap>("../../%WorldTileMap");

    StructureTilePlacer _tilePlacer;

    public override void _Notification(int what)
    {
        if (Engine.IsEditorHint())
            EditorNotification(what);
        else
            Notification(what);
    }

    #region editor

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();
        if (TileGraphicsDef is null)
            warnings.Add("No TileGraphicsDef set");
        if (WorldTileMap is null)
            warnings.Add("No WorldTileMap found");
        return warnings.ToArray();
    }

    void EditorNotification(int what)
    {
        switch ((long)what)
        {
            case NotificationReady:
                SetNotifyTransform(true);
                break;
            case NotificationTransformChanged:
                EditorUpdate();
                break;
            case NotificationEditorPreSave:
                EditorCleanup();
                break;
            case NotificationEditorPostSave:
                EditorUpdate();
                break;
            case NotificationExitTree:
                EditorCleanup();
                break;
        }
    }

    void EditorUpdate()
    {
        _tilePlacer?.Remove();
        _tilePlacer = EditorTryCreateTilePlacer();
        _tilePlacer?.ToNormalGraphics();
        UpdateConfigurationWarnings();
    }

    void EditorCleanup()
    {
        _tilePlacer?.Remove();
        _tilePlacer = null;
    }

    StructureTilePlacer EditorTryCreateTilePlacer()
    {
        if (TileGraphicsDef is not null && WorldTileMap is not null)
            return new(TileGraphicsDef, WorldTileMap, this);
        else
            return null;
    }

    #endregion

    void Notification(int what)
    {
        switch ((long)what)
        {
            case NotificationReady:
                Setup();
                break;
            case NotificationExitTree:
                Cleanup();
                break;
        }
    }

    void Setup()
    {
        _tilePlacer = new(TileGraphicsDef, WorldTileMap, this);
        _tilePlacer.ToBlueprintGraphics();

        _structureGraphics = GetNode<StructureGraphics>("../StructureGraphics");
        _structureGraphics.OnConstructionCompleted += OnConstructionCompleted;
    }

    void Cleanup()
    {
        _tilePlacer.Remove();

        _structureGraphics.OnConstructionCompleted -= OnConstructionCompleted;
    }

    void OnConstructionCompleted() => _tilePlacer.ToNormalGraphics();
}
