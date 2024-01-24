#nullable enable
using Godot;

namespace LogicGraphs;

[Tool, GlobalClass]
public partial class LogicConnection : Resource
{
    [Export]
    public int SourceNodeIndex;

    [Export]
    public StringName Signal = null!;

    [Export]
    public int TargetNodeIndex;

    [Export]
    public StringName Method = null!;
}
