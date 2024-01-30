#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class Print : LogicNode
{
    [Signal]
    public delegate void PrintedEventHandler();

    [Export]
    string _message = string.Empty;

    public void Execute()
    {
        GD.Print(_message);
        EmitSignal(SignalName.Printed);
    }
}
