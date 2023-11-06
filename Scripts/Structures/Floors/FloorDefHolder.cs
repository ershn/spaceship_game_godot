using Godot;

[GlobalClass]
public partial class FloorDefHolder : StructureDefHolder, ITemplate<StructureDef>
{
    public void Template(StructureDef structureDef)
    {
        FloorDef = (FloorDef)structureDef;
    }

    public override StructureDef StructureDef => FloorDef;

    [Export]
    public FloorDef FloorDef;
}
