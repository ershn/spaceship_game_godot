#nullable enable
using Godot;

namespace LogicGraphs;

[Tool, GlobalClass, Internal]
public partial class LogicGraphEntrypoint : LogicNode
{
    [Signal]
    public delegate void StartedEventHandler();

    [Signal]
    public delegate void ProcessedEventHandler();

    [Internal]
    public void Start() => EmitSignal(SignalName.Started);

    [Internal]
    public void Process() => EmitSignal(SignalName.Processed);
}
