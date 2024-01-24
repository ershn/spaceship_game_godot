#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class Sequence : LogicNode
{
    [Signal]
    public delegate void FirstEventHandler();

    [Signal]
    public delegate void SecondEventHandler();

    public void Execute()
    {
        EmitSignal(SignalName.First);
        EmitSignal(SignalName.Second);
    }
}
