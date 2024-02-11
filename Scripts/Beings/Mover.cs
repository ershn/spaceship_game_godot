using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class Mover : NavigationAgent2D
{
    [Signal]
    public delegate void MovedEventHandler(Vector2 direction);

    [Signal]
    public delegate void StoppedEventHandler();

    [Export]
    CharacterBody2D _characterBody2D;

    [Export]
    float _movementSpeed = 100f;

    bool _ready;
    TaskCompletionSource _taskCompletionSource;
    CancellationToken _cancellationToken;

    public async Task MoveTo(Vector2 target, CancellationToken ct)
    {
        if (!_ready)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        }

        _taskCompletionSource = new TaskCompletionSource();
        _cancellationToken = ct;

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

        var currentPosition = _characterBody2D.GlobalTransform.Origin;
        var nextPathPosition = GetNextPathPosition();

        var newDirection = (nextPathPosition - currentPosition).Normalized();
        _characterBody2D.Velocity = newDirection * _movementSpeed;

        _characterBody2D.MoveAndSlide();
        EmitSignal(SignalName.Moved, _characterBody2D.GetLastMotion().Normalized());

        if (IsTargetReached())
        {
            _taskCompletionSource.SetResult();
            EmitSignal(SignalName.Stopped);
        }
    }
}
