#nullable enable
using Godot;

[GlobalClass]
public partial class CloneAnimator : Node
{
    [Export]
    Mover _mover = null!;

    [Export]
    FoodConsumer _foodConsumer = null!;

    [Export]
    Death _death = null!;

    [Export]
    AnimationTree _animationTree = null!;

    AnimationNodeStateMachinePlayback _playback = null!;
    float _movingAnimSpeed;

    Vector2 _direction = Vector2.Down;

    public override void _Ready()
    {
        _playback = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
        _movingAnimSpeed = (float)_animationTree.Get("parameters/moving/speed/scale");

        _mover.Moved += ToMoving;
        _mover.Stopped += ToIdling;
        _foodConsumer.StartedConsuming += ToEating;
        _foodConsumer.StoppedConsuming += ToIdling;
        _death.Died += ToDying;
    }

    void ToMoving(Vector2 direction, float speedRatio)
    {
        _direction = direction;

        _animationTree.Set("parameters/moving/direction/blend_position", _direction);
        _animationTree.Set("parameters/moving/speed/scale", _movingAnimSpeed * speedRatio);
        _playback.Travel("moving");
    }

    void ToIdling()
    {
        _animationTree.Set("parameters/idling/blend_position", _direction);
        _playback.Travel("idling");
    }

    void ToEating()
    {
        _animationTree.Set("parameters/eating/blend_position", _direction);
        _playback.Travel("eating");
    }

    void ToDying()
    {
        _mover.Moved -= ToMoving;
        _mover.Stopped -= ToIdling;
        _foodConsumer.StartedConsuming -= ToEating;
        _foodConsumer.StoppedConsuming -= ToIdling;

        _playback.Travel("dying");
    }
}
