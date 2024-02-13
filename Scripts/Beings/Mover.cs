using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class Mover : NavigationAgent2D
{
    [Signal]
    public delegate void MovedEventHandler(Vector2 direction, float speedRatio);

    [Signal]
    public delegate void StoppedEventHandler();

    [Export]
    CharacterBody2D _characterBody2D;

    [Export]
    float _movementSpeed = 100f;

    bool _ready;
    TaskCompletionSource _taskCompletionSource;
    CancellationToken _cancellationToken;
    double _elapsedTime;

    public async Task MoveTo(Vector2 target, CancellationToken ct)
    {
        if (!_ready)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        }

        _taskCompletionSource = new TaskCompletionSource();
        _cancellationToken = ct;
        _elapsedTime = 0d;

        TargetPosition = target;
        if (!IsTargetReachable())
            _taskCompletionSource.SetCanceled(_cancellationToken);

        await _taskCompletionSource.Task;
    }

    public override void _PhysicsProcess(double delta)
    {
        _ready = true;
        if (_taskCompletionSource is null || _taskCompletionSource.Task.IsCompleted)
            return;

        if (_cancellationToken.IsCancellationRequested)
        {
            _taskCompletionSource.SetCanceled(_cancellationToken);
            EmitSignal(SignalName.Stopped);
            return;
        }

        _elapsedTime += delta;

        var currentPosition = _characterBody2D.GlobalTransform.Origin;
        var nextPathPosition = GetNextPathPosition();

        var newDirection = (nextPathPosition - currentPosition).Normalized();
        var newSpeed = GetMovementSpeed();
        _characterBody2D.Velocity = newDirection * newSpeed;

        _characterBody2D.MoveAndSlide();

        var finalDirection = _characterBody2D.GetLastMotion().Normalized();
        var speedRatio = newSpeed / _movementSpeed;
        EmitSignal(SignalName.Moved, finalDirection, speedRatio);

        if (IsTargetReached())
        {
            _taskCompletionSource.SetResult();
            EmitSignal(SignalName.Stopped);
        }
    }

    float GetMovementSpeed()
    {
        var weight = Mathf.Min((float)(_elapsedTime / 1.5d), 1f);
        return Mathf.Lerp(_movementSpeed * .5f, _movementSpeed, Mathf.Ease(weight, .25f));
    }
}
