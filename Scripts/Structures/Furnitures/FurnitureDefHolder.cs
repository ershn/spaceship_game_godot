using Godot;

[Tool, GlobalClass]
public partial class FurnitureDefHolder : StructureDefHolder, ITemplate<StructureDef>
{
    public void Template(StructureDef structureDef)
    {
        FurnitureDef = (FurnitureDef)structureDef;
    }

    public override StructureDef StructureDef => FurnitureDef;

    [Export]
    public FurnitureDef FurnitureDef;
}
