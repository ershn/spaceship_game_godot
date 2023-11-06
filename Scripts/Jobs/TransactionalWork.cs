using System.Diagnostics;
using Godot;

public abstract partial class TransactionalWork : Node, IWork
{
    enum Phase
    {
        Pending,
        Started,
        Completed
    }

    [Signal]
    public delegate void OnWorkStartedEventHandler();

    [Signal]
    public delegate void OnWorkProgressEventHandler(float progress);

    [Signal]
    public delegate void OnWorkCompletedEventHandler();

    [Signal]
    public delegate void OnWorkResetEventHandler();

    protected abstract double RequiredTime { get; }

    Phase _phase = Phase.Pending;
    double _currentTime;

    public bool Work(double time)
    {
        Debug.Assert(_phase != Phase.Completed);

        if (_phase == Phase.Pending)
        {
            _phase = Phase.Started;
            EmitSignal(SignalName.OnWorkStarted);
        }

        _currentTime += time;

        var progress = Mathf.Clamp((float)_currentTime / (float)RequiredTime, 0f, 1f);
        EmitSignal(SignalName.OnWorkProgress, progress);

        if (Mathf.IsEqualApprox(progress, 1f))
        {
            _phase = Phase.Completed;
            EmitSignal(SignalName.OnWorkCompleted);
        }

        return _phase == Phase.Completed;
    }

    public void Reset()
    {
        _phase = Phase.Pending;
        _currentTime = 0d;
        EmitSignal(SignalName.OnWorkReset);
    }

    public Vector2 GlobalPosition => GetOwner<Node2D>().GlobalPosition;

    public Vector2 Position => GetOwner<Node2D>().Position;
}
