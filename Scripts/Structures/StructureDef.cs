using Godot;

public abstract partial class StructureDef : EntityDef, IWorldLayerGet
{
    public abstract WorldLayer WorldLayer { get; }

    [ExportGroup("Graphics")]
    [Export]
    public StructureGraphicsDef StructureGraphicsDef;

    [ExportGroup("Construction")]
    [Export]
    public ItemDefAmount[] ComponentAmounts;

    [Export]
    public float ConstructionTime = 10f;

    [Export]
    public float DeconstructionTimeMultiplier = .5f;

    // [ExportGroup("Status")]
    // public bool SetupRequired = false;

    [ExportGroup("Health")]
    [Export]
    public uint MaxHealthPoints = 100;

    // [ExportGroup("Resource processing")]
    // public StateGraphAsset ResourceProcessor;

    public abstract bool IsConstructibleAt(
        Grids grids,
        Vector2I cellPosition,
        bool ignoreExisting = false
    );
}
