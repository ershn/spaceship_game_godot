#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class Forward1 : LogicNode
{
    [Export]
    NodePath _nodePath = null!;

    [Export]
    StringName _method = null!;

    Node _node = null!;

    protected override void _Ready()
    {
        _node = Entity.GetNode(_nodePath);
    }

    public void Execute(Variant param1)
    {
        _node.Call(_method, param1);
    }
}
