using Godot;

[GlobalClass]
public partial class DeconstructionWork : TransactionalWork
{
    StructureDef _structureDef;

    protected override double RequiredTime =>
        _structureDef.ConstructionTime * _structureDef.DeconstructionTimeMultiplier;

    public override void _Ready()
    {
        _structureDef = GetNode<StructureDefHolder>("../DefHolder").StructureDef;
    }
}
