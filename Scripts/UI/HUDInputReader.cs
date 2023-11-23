using Godot;

[GlobalClass]
public partial class HUDInputReader : Node
{
    [Signal]
    public delegate void MouseClickEventHandler(Vector2 position);

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                // TODO: add the world position
                EmitSignal(SignalName.MouseClick, mouseButton.Position);
            }
        }
    }
}
