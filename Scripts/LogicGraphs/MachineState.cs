#nullable enable
using Godot;

namespace LogicGraphs;

[Tool, GlobalClass, InputNode]
public partial class MachineState : LogicNode
{
    [Signal]
    public delegate void EnteredEventHandler();

    [Signal]
    public delegate void ProcessedEventHandler();

    [Signal]
    public delegate void ExitedEventHandler();

    [Export]
    public string Name = string.Empty;

    [Internal]
    public void Enter() => EmitSignal(SignalName.Entered);

    [Internal]
    public void Process() => EmitSignal(SignalName.Processed);

    [Internal]
    public void Exit() => EmitSignal(SignalName.Exited);
}
