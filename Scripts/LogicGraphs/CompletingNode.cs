#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public abstract partial class CompletingNode : LogicNode
{
    [Signal]
    public delegate void ProcessedEventHandler(bool completed);

    [Signal]
    public delegate void CompletedEventHandler();

    [Export]
    bool _resetOnCompletion = true;

    public void Process()
    {
        var completed = _Process();
        EmitSignal(SignalName.Processed, completed);
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
