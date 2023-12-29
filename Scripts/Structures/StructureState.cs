public abstract partial class StructureState : EntityState
{
    public override EntityDef EntityDef => StructureDef;

    public abstract StructureDef StructureDef { get; }
}
