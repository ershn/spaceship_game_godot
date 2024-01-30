#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class WithArguments : LogicNode
{
    [Signal]
    public delegate void CalledEventHandler();

    [Export]
    Godot.Collections.Array<Variant> _arguments = [];

    Variant[] _signalArguments = null!;

    protected override void _Ready()
    {
        _signalArguments = [.. _arguments];
    }

    public void Execute()
    {
        EmitSignal(SignalName.Called, _signalArguments);
    }
}
