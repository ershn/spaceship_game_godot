using Godot;

[GlobalClass]
public partial class CloneAnimator : Node
{
    [Export]
    Mover _mover;

    [Export]
    Death _death;

    [Export]
    AnimationTree _animationTree;

    public override void _Ready()
    {
        _mover.Moved += OnMoved;
        _mover.Stopped += OnStopped;
        _death.OnDeath += OnDeath;
    }

    void OnMoved(Vector2 direction)
    {
        _animationTree.Set("parameters/idle/blend_position", direction);
        _animationTree.Set("parameters/move/blend_position", direction);

        _animationTree.Set("parameters/conditions/is_idling", false);
        _animationTree.Set("parameters/conditions/is_moving", true);
    }

    void OnStopped()
    {
        _animationTree.Set("parameters/conditions/is_idling", true);
        _animationTree.Set("parameters/conditions/is_moving", false);
    }

    void OnDeath()
    {
        _animationTree.Set("parameters/conditions/died", true);
    }
}
