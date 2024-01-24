#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class StateMachine : LogicNode
{
    [Export]
    Godot.Collections.Dictionary<string, MachineState> _states = [];

    [Export]
    MachineState? _currentState;

    public void Start() { }

    public void Update() { }

    public void SwitchTo(string stateName) { }
}
