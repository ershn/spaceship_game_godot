using Godot;

[GlobalClass]
public partial class ConstructionWork : TransactionalWork
{
    StructureDef _structureDef;

    protected override double RequiredTime => _structureDef.ConstructionTime;

    public override void _Ready()
    {
        _structureDef = GetNode<StructureDefHolder>("../DefHolder").StructureDef;
    }
}
