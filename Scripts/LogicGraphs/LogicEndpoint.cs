#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class LogicEndpoint : Resource
{
    [Export]
    public int NodeIndex;

    [Export]
    public StringName Member = null!;
}
