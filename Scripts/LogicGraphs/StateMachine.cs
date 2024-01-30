#nullable enable
using System.Collections.Generic;
using Godot;

namespace LogicGraphs;

[Tool]
public partial class StateMachine : LogicNode
{
    [Export, OutputNodes]
    int[] _stateNodeIndexes = [];

    readonly Dictionary<string, MachineState> _states = [];

    MachineState _currentState = null!;

    [Export]
    string _currentStateName = string.Empty;

    [Export]
    bool _currentStateEntered;

    bool _currentStateExited;

    MachineState? _nextState;

    [Export]
    string _nextStateName = string.Empty;

    protected override void _Ready()
    {
        foreach (var index in _stateNodeIndexes)
        {
            var state = (MachineState)Graph.Nodes[index];
            if (state.Name.Length > 0)
                _states.Add(state.Name, state);
            else
                GD.PushError($"{nameof(MachineState)} name not set");
        }

        if (_currentStateName.Length > 0)
            _currentState = _states[_currentStateName];
        else
            GD.PushError($"{nameof(StateMachine)} current state name not set");

        if (_nextStateName.Length > 0)
            _nextState = _states[_nextStateName];
    }

    public void Process()
    {
        if (_nextState is not null)
        {
            _currentState = _nextState;
            _currentStateName = _nextStateName;
            _currentStateEntered = false;
            _currentStateExited = false;
            _nextState = null;
            _nextStateName = string.Empty;
        }
        if (!_currentStateEntered)
        {
            _currentStateEntered = true;
            _currentState.Enter();
            if (_nextState is not null)
                return;
        }
        _currentState.Process();
    }

    public void SwitchTo(string stateName)
    {
        _nextState = _states[stateName];
        _nextStateName = _nextState.Name;
        if (!_currentStateExited)
        {
            _currentStateExited = true;
            _currentState.Exit();
        }
    }
}
