using Godot;

[Tool]
public partial class FurnitureState : StructureState
{
    public override StructureDef StructureDef => FurnitureDef;

    [Export]
    public FurnitureDef FurnitureDef;

    FurnitureState() { }

    public FurnitureState(FurnitureDef furnitureDef)
    {
        FurnitureDef = furnitureDef;
    }

    public override void Initialize(Node2D _furniture) { }

    public override void EditorInitialize(Node2D _furniture) { }

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        if ((string)property["name"] == nameof(FurnitureDef))
            property["usage"] = (long)(PropertyUsageFlags.ReadOnly | PropertyUsageFlags.Storage);
    }
}
