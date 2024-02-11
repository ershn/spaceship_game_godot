#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public abstract partial class ProgressingNode : LogicNode
{
    [Signal]
    public delegate void ProcessedEventHandler(float ratio);

    [Signal]
    public delegate void CompletedEventHandler();

    [Export]
    bool _resetOnCompletion = true;

    public void Process()
    {
        var ratio = _Process();
        EmitSignal(SignalName.Processed, ratio);
        if (ratio >= 0.99f)
        {
            EmitSignal(SignalName.Completed);
            if (_resetOnCompletion)
                _Reset();
        }
    }

    protected abstract float _Process();

    public void Reset() => _Reset();

    protected virtual void _Reset() { }
}
