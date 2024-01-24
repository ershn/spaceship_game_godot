#if TOOLS
#nullable enable
using Godot;

namespace LogicGraphs.Editor;

[Tool]
public partial class Plugin : EditorPlugin
{
    const string EditorScenePath = "res://addons/LogicGraphEditor/graph_editor.tscn";

    const string EditorName = "LogicGraph";

    GraphEditor _graphEditor = null!;

    public override void _EnterTree()
    {
        _graphEditor = GD.Load<PackedScene>(EditorScenePath).Instantiate<GraphEditor>();
        _graphEditor.GetNode<GraphEditorContextMenu>("%ContextMenu").Initialize();
        EditorInterface.Singleton.GetEditorMainScreen().AddChild(_graphEditor);
        _graphEditor.Hide();
    }

    public override void _ExitTree()
    {
        _graphEditor.QueueFree();
        _graphEditor = null!;
    }

    public override bool _HasMainScreen() => true;

    public override string _GetPluginName() => EditorName;

    public override Texture2D _GetPluginIcon() =>
        EditorInterface.Singleton.GetEditorTheme().GetIcon("CurveEdit", "EditorIcons");

    public override void _MakeVisible(bool visible)
    {
        _graphEditor.Visible = visible;
    }

    public override bool _Handles(GodotObject @object) =>
        @object is LogicGraph || @object is LogicNode;

    public override void _Edit(GodotObject? @object)
    {
        if (@object is LogicGraph logicGraph)
        {
            if (_graphEditor.IsGraphLoaded)
            {
                if (!_graphEditor.IsGraphSaved)
                    _graphEditor.SaveGraph();
                _graphEditor.UnloadGraph();
            }
            _graphEditor.LoadGraph(logicGraph);
        }
    }

    public override string _GetUnsavedStatus(string scenePath)
    {
        if (scenePath.Length != 0)
            return string.Empty;

        if (_graphEditor.IsGraphLoaded && !_graphEditor.IsGraphSaved)
            return "The logic graph is unsaved.";
        else
            return string.Empty;
    }

    public override void _SaveExternalData()
    {
        if (_graphEditor.IsGraphLoaded && !_graphEditor.IsGraphSaved)
            _graphEditor.SaveGraph();
    }
}
#endif
