#if TOOLS
#nullable enable
using Godot;
using GDC = Godot.Collections;

namespace LogicGraphs.Editor;

[Tool]
public partial class GraphEditor : GraphEdit
{
    [Export]
    PopupMenu _contextMenu = null!;

    GraphEditorDB? _db;

    public bool IsGraphLoaded => _db is not null;

    public void LoadGraph(LogicGraph logicGraph)
    {
        _db = new GraphEditorDB(logicGraph);

        foreach (var editorNode in _db.GetNodes())
            AddChild(editorNode);
        foreach (var (fromNode, fromPort, toNode, toPort) in _db.GetOutgoingConnections())
            ConnectNode(fromNode.Name, fromPort, toNode.Name, toPort);

        Zoom = _db.EditorZoom;
        ScrollOffset = _db.EditorScrollOffset;
    }

    public void UnloadGraph()
    {
        ClearConnections();
        foreach (var editorNode in _db!.GetNodes())
            editorNode.QueueFree();

        _db.Free();
        _db = null;
    }

    public bool IsGraphSaved => _db!.Saved;

    public void SaveGraph()
    {
        _db!.Save();
    }

    public void AddNode(GraphEditorNode editorNode)
    {
        _db!.AddNode(editorNode);
        AddChild(editorNode);
    }

    public void RemoveNode(GraphEditorNode editorNode)
    {
        foreach (var (fromNode, fromPort, toNode, toPort) in _db!.GetConnections(editorNode))
            DisconnectNode(fromNode.Name, fromPort, toNode.Name, toPort);
        _db.RemoveNode(editorNode);
        editorNode.QueueFree();
    }

    void AddConnection(GraphEditorNode fromNode, int fromPort, GraphEditorNode toNode, int toPort)
    {
        _db!.AddConnection(fromNode, fromPort, toNode, toPort);
        ConnectNode(fromNode.Name, fromPort, toNode.Name, toPort);
    }

    void RemoveConnection(
        GraphEditorNode fromNode,
        int fromPort,
        GraphEditorNode toNode,
        int toPort
    )
    {
        _db!.RemoveConnection(fromNode, fromPort, toNode, toPort);
        DisconnectNode(fromNode.Name, fromPort, toNode.Name, toPort);
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (_db is null)
            return;

        if (
            @event is InputEventMouseButton mouseButton
            && mouseButton.ButtonIndex == MouseButton.Right
            && mouseButton.Pressed
        )
        {
            var screenPosition = GetScreenTransform() * mouseButton.Position;
            _contextMenu.Position = (Vector2I)screenPosition;
            _contextMenu.Show();
        }
    }

    void OnConnectionRequest(StringName fromNode, int fromPort, StringName toNode, int toPort)
    {
        AddConnection(
            GetNode<GraphEditorNode>(fromNode.ToString()),
            fromPort,
            GetNode<GraphEditorNode>(toNode.ToString()),
            toPort
        );
    }

    void OnDisconnectionRequest(StringName fromNode, int fromPort, StringName toNode, int toPort)
    {
        RemoveConnection(
            GetNode<GraphEditorNode>(fromNode.ToString()),
            fromPort,
            GetNode<GraphEditorNode>(toNode.ToString()),
            toPort
        );
    }

    void OnDeleteNodesRequest(GDC.Array<StringName> nodeNames)
    {
        foreach (var nodeName in nodeNames)
            RemoveNode(GetNode<GraphEditorNode>(nodeName.ToString()));
    }

    void OnScrollOffsetChanged(Vector2 scrollOffset)
    {
        _db?.SetEditorState(scrollOffset, Zoom);
    }
}
#endif
