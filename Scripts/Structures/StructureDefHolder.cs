using Godot;

[GlobalClass]
public abstract partial class StructureDefHolder
    : EntityDefHolder,
        IWorldLayerGet,
        IHealthHolderConf
{
    public override EntityDef EntityDef => StructureDef;

    public abstract StructureDef StructureDef { get; }

    public WorldLayer WorldLayer => StructureDef.WorldLayer;

    public uint MaxHealthPoints => StructureDef.MaxHealthPoints;
}
