#nullable enable
using Godot;

namespace LogicGraphs;

[GlobalClass]
public partial class LogicExecutor : Node
{
    [Export]
    LogicGraph _logicGraphTemplate = null!;

    LogicGraph? _logicGraph;

    public void Start()
    {
        _logicGraph = _logicGraphTemplate.Clone();
        _logicGraph.Setup(GetParent<Node2D>());
        _logicGraph.Start();
    }

    public override void _Process(double _delta)
    {
        _logicGraph?.Process();
    }
}
