#if TOOLS
using System;
using System.Collections.Generic;
using Godot;

namespace EntityDrawer;

[Tool]
public partial class Plugin : EditorPlugin, ISerializationListener
{
    const string DefsPath = "res://Definitions/";

    const string PanelScenePath = "res://addons/EntityDrawer/panel.tscn";
    const string EntityDefButtonScenePath = "res://addons/EntityDrawer/entity_def_button.tscn";
    const string EntityStatesDirPath = "res://addons/EntityDrawer/EntityStates/";

    const string PanelName = "Entity Drawer";

    EditorInterface _editorInterface;

    Control _panel;
    Button _panelButton;

    Godot.Collections.Dictionary<EntityDef, EntityDefButton> _entityButtons;

    MouseButton? _pressedMouseButton;
    Vector2I? _previousCoord;

    TileMap _tileMap;
    EntityPlacer _entityPlacer;

    TileMap TileMap
    {
        get => _tileMap;
        set
        {
            _tileMap = value;
            _entityPlacer = _tileMap is null ? null : new EntityPlacer(_tileMap);
        }
    }

    EntityDef _entityDef;
    EntityState _entityState;

    public override void _EnterTree()
    {
        _editorInterface = EditorInterface.Singleton;

        _panel = GD.Load<PackedScene>(PanelScenePath).Instantiate<Control>();

        _panelButton = AddControlToBottomPanel(_panel, PanelName);
        _panelButton.Hide();

        _panel.GetNode<Button>("%ReloadButton").Pressed += LoadDefs;

        OnFilesystemReady(LoadDefs);
    }

    public override void _ExitTree()
    {
        _panel.GetNode<Button>("%ReloadButton").Pressed -= LoadDefs;

        _panelButton = null;
        RemoveControlFromBottomPanel(_panel);

        _panel.QueueFree();
    }

    public override bool _Handles(GodotObject @object)
    {
        return @object is TileMap;
    }

    public override void _Edit(GodotObject @object)
    {
        TileMap = (TileMap)@object;
    }

    public override void _MakeVisible(bool visible)
    {
        _panelButton.Visible = visible;
        if (visible)
            MakeBottomPanelItemVisible(_panel);
        else
            HideBottomPanel();
    }

    public override bool _ForwardCanvasGuiInput(InputEvent @event)
    {
        if (!_panel.Visible)
            return false;

        switch (@event)
        {
            case InputEventMouseButton mouseButton:
                switch (mouseButton.ButtonIndex)
                {
                    case MouseButton.Left:
                    case MouseButton.Right:
                    case MouseButton.Xbutton2:
                        _pressedMouseButton = mouseButton.Pressed ? mouseButton.ButtonIndex : null;
                        break;
                    default:
                        return false;
                }
                break;
            case InputEventMouseMotion:
                if (_pressedMouseButton is null)
                    return false;
                break;
            default:
                return false;
        }

        HandleMouseInput(_pressedMouseButton);
        return true;
    }

    void HandleMouseInput(MouseButton? button)
    {
        if (button is null)
        {
            _previousCoord = null;
            return;
        }

        var coord = TileMap.LocalToMap(TileMap.GetLocalMousePosition());
        if (coord == _previousCoord)
            return;

        switch (button)
        {
            case MouseButton.Left:
                Paint(coord);
                break;
            case MouseButton.Right:
                Erase(coord);
                break;
            case MouseButton.Xbutton2:
                Pick(coord);
                break;
        }
        _previousCoord = coord;
    }

    void LoadDefs()
    {
        _entityButtons = new();

        var defsGrid = _panel.GetNode<GridContainer>("%Palette");
        foreach (var child in defsGrid.GetChildren())
            child.Free();

        var defsButtonGroup = new ButtonGroup();
        foreach (var (path, defs) in LoadDefsAtPath(DefsPath))
            AddDefsToGrid(path, defs, defsButtonGroup, defsGrid, _entityButtons);

        if (_entityDef is not null)
            TrySelectDefFromGrid(_entityDef);
    }

    Dictionary<string, List<EntityDef>> LoadDefsAtPath(string dirPath)
    {
        var defPaths = new Dictionary<string, List<EntityDef>>();

        void LoadFrom(EditorFileSystemDirectory dir, List<string> parentNames)
        {
            if (dir.GetFileCount() > 0)
            {
                var resources = new List<EntityDef>();
                for (int i = 0; i < dir.GetFileCount(); i++)
                    resources.Add(GD.Load<EntityDef>(dir.GetFilePath(i)));
                defPaths.Add(string.Join('/', parentNames), resources);
            }
            for (int i = 0; i < dir.GetSubdirCount(); i++)
            {
                var subdir = dir.GetSubdir(i);
                LoadFrom(subdir, new List<string>(parentNames) { subdir.GetName() });
            }
        }

        var filesystem = _editorInterface.GetResourceFilesystem();
        var dir = filesystem.GetFilesystemPath(dirPath);
        LoadFrom(dir, new List<string>());

        return defPaths;
    }

    void AddDefsToGrid(
        string path,
        IEnumerable<EntityDef> defs,
        ButtonGroup buttonGroup,
        GridContainer grid,
        Godot.Collections.Dictionary<EntityDef, EntityDefButton> entityButtons
    )
    {
        var label = new Label() { Text = path, HorizontalAlignment = HorizontalAlignment.Right };
        var flowContainer = new HFlowContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };

        var buttonScene = GD.Load<PackedScene>(EntityDefButtonScenePath);
        foreach (var def in defs)
        {
            var button = buttonScene.Instantiate<EntityDefButton>();
            button.ButtonGroup = buttonGroup;
            button.EntityDef = def;
            button.Selected += SelectDef;
            flowContainer.AddChild(button);
            entityButtons.Add(def, button);
        }

        grid.AddChild(label);
        grid.AddChild(flowContainer);
    }

    void TrySelectDefFromGrid(EntityDef def)
    {
        if (_entityButtons.TryGetValue(def, out var button))
            button.ButtonPressed = true;
    }

    void SelectDef(EntityDef def)
    {
        _entityDef = def;
        _entityState = GetDefState(def);

        _editorInterface.EditResource(_entityState);
    }

    EntityState GetDefState(EntityDef def)
    {
        var statePath = EntityStatesDirPath + def.ResourcePath.GetFile();
        if (FileAccess.FileExists(statePath))
            return GD.Load<EntityState>(statePath);

        var state = def.NewState();
        state.ResourcePath = statePath;

        var error = ResourceSaver.Save(state);
        if (error != Error.Ok)
            GD.PushError("Failed to save entity state at ", statePath, ": ", error);

        return state;
    }

    void Paint(Vector2I coord)
    {
        if (_entityDef is null)
            return;

        if (_entityPlacer.TryPlaceEntity(coord, _entityDef, _entityState))
            _editorInterface.MarkSceneAsUnsaved();
    }

    void Erase(Vector2I coord)
    {
        if (_entityPlacer.RemoveEntities(coord))
            _editorInterface.MarkSceneAsUnsaved();
    }

    void Pick(Vector2I coord)
    {
        if (_entityPlacer.TryPickEntity(coord, out var entityDef))
            TrySelectDefFromGrid(entityDef);
    }

    void OnFilesystemReady(Action action)
    {
        var filesystem = _editorInterface.GetResourceFilesystem();
        if (filesystem.GetFilesystem().GetSubdirCount() == 0)
        {
            filesystem.Connect(
                EditorFileSystem.SignalName.FilesystemChanged,
                Callable.From(action),
                (uint)ConnectFlags.OneShot
            );
        }
        else
            action();
    }

    // HACK: necessary to avoid a display bug
    public void OnAfterDeserialize() => Callable.From(LoadDefs).CallDeferred();

    public void OnBeforeSerialize() { }
}
#endif
