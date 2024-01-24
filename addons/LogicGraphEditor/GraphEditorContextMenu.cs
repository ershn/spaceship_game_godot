#if TOOLS
#nullable enable
using System.Linq;
using Godot;

namespace LogicGraphs.Editor;

[Tool]
public partial class GraphEditorContextMenu : PopupMenu, ISerializationListener
{
    [Export]
    GraphEditor _graphEditor = null!;

    LogicNodeDef[] _logicNodeDefs = null!;

    bool _initialized;

    public void Initialize()
    {
        _logicNodeDefs = LogicNodeDef.GetDefs().Where(def => !def.Internal).ToArray();
        foreach (var def in _logicNodeDefs)
            AddItem(def.Name);

        _initialized = true;
    }

    void Reset()
    {
        if (_initialized)
        {
            Clear();
            Size = new Vector2I(0, 0);
            Initialize();
        }
    }

    void ItemPressed(int index)
    {
        var menuPosition = _graphEditor.GetScreenTransform().AffineInverse() * Position;
        var editorNodePosition = (menuPosition + _graphEditor.ScrollOffset) / _graphEditor.Zoom;
        var logicNode = _logicNodeDefs[index].Instantiate();
        logicNode.GraphEditorPosition = editorNodePosition;
        _graphEditor.AddNode(new GraphEditorNode(logicNode));
    }

    public void OnAfterDeserialize() => Reset();

    public void OnBeforeSerialize() { }
}
#endif
