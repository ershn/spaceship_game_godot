using Godot;

[GlobalClass]
public partial class FloorConstructor : Node
{
    public override void _Ready()
    {
        _ = GetNode<StructureConstructor>("../StructureConstructor").Construct();
    }
}
