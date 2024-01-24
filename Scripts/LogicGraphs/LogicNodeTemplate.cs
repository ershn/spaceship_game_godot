#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public abstract partial class LogicNodeTemplate : LogicNode
{
    [Signal]
    public delegate void ProcessedEventHandler();

    [Signal]
    public delegate void CompletedEventHandler();

    [Export]
    bool _resetOnCompletion = true;

    public void Process()
    {
        var completed = _Process();
        EmitSignal(SignalName.Processed);
        if (completed)
        {
            EmitSignal(SignalName.Completed);
            if (_resetOnCompletion)
                _Reset();
        }
    }

    protected abstract bool _Process();

    public void Reset() => _Reset();

    protected virtual void _Reset() { }
}
