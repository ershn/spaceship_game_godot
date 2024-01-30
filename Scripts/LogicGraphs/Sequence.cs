#nullable enable
using System.Linq;
using Godot;

namespace LogicGraphs;

[Tool]
public partial class Sequence : LogicNode
{
    readonly record struct Target(LogicNode Node, StringName Method) { }

    [Export, OutputMethods]
    LogicEndpoint[] _targetMethodReferences = [];

    Target[] _targets = null!;

    protected override void _Ready()
    {
        _targets = _targetMethodReferences
            .Select(reference => new Target(Graph.Nodes[reference.NodeIndex], reference.Member))
            .ToArray();
    }

    public void Execute()
    {
        foreach (var target in _targets)
            target.Node.Call(target.Method);
    }
}
