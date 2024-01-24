#if TOOLS
#nullable enable
using Godot;

namespace LogicGraphs.Editor;

public partial class ConnectionEndpoint : GodotObject
{
    public GraphEditorNode EditorNode { get; private set; }
    public int Port { get; private set; }

#pragma warning disable CS8618
    ConnectionEndpoint() { }
#pragma warning restore CS8618

    public ConnectionEndpoint(GraphEditorNode editorNode, int port)
    {
        EditorNode = editorNode;
        Port = port;
    }
}
#endif
