#nullable enable
using Godot;

namespace LogicGraphs;

[Tool, GlobalClass]
public partial class LogicGraph : Resource
{
    [Export]
    public LogicGraphEntrypoint Entrypoint = new();

    [Export]
    public LogicNode[] Nodes = [];

    [Export]
    public LogicConnection[] Connections = [];

    public LogicGraph Clone()
    {
        var clone = (LogicGraph)Duplicate();
        clone.Entrypoint = (LogicGraphEntrypoint)Entrypoint.Clone();
        for (int index = 0; index < Nodes.Length; index++)
            clone.Nodes[index] = Nodes[index].Clone();
        return clone;
    }

    public void Setup(Node2D entity)
    {
        Entrypoint.Setup(this, entity);
        foreach (var node in Nodes)
            node.Setup(this, entity);

        foreach (var connection in Connections)
        {
            GetNode(connection.SourceNodeIndex)
                .Connect(
                    connection.Signal,
                    new Callable(GetNode(connection.TargetNodeIndex), connection.Method)
                );
        }

        Entrypoint.Ready();
        foreach (var node in Nodes)
            node.Ready();
    }

    LogicNode GetNode(int index) => index >= 0 ? Nodes[index] : Entrypoint;

    public void Start() => Entrypoint.Start();

    public void Process() => Entrypoint.Process();

#if TOOLS
    [Export]
    public Vector2 GraphEditorScrollOffset;

    [Export]
    public float GraphEditorZoom = 1f;

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        var name = (string)property["name"];
        if (name == nameof(GraphEditorScrollOffset) || name == nameof(GraphEditorZoom))
            property["usage"] = (long)PropertyUsageFlags.NoEditor;
    }
#endif
}
