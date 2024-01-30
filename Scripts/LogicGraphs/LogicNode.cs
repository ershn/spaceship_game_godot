#nullable enable
using Godot;

namespace LogicGraphs;

[Tool, GlobalClass]
public partial class LogicNode : Resource
{
    [Internal]
    public virtual LogicNode Clone() => (LogicNode)Duplicate();

    protected LogicGraph Graph = null!;
    protected Node2D Entity = null!;

    [Internal]
    public void Setup(LogicGraph graph, Node2D entity)
    {
        Graph = graph;
        Entity = entity;
    }

    [Internal]
    public void Ready() => _Ready();

    protected virtual void _Ready() { }

#if TOOLS
    [Export]
    public Vector2 GraphEditorPosition;

    [Internal]
    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        if ((string)property["name"] == nameof(GraphEditorPosition))
            property["usage"] = (long)PropertyUsageFlags.NoEditor;
    }
#endif
}
