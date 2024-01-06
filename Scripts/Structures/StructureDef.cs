using Godot;

[GlobalClass]
public abstract partial class StructureDef : EntityDef, IWorldLayerGet
{
    public abstract WorldLayer WorldLayer { get; }

    [ExportGroup("Graphics")]
    public override Texture2D PreviewSprite => StructureGraphicsDef?.PreviewSprite;

    [Export]
    public StructureGraphicsDef StructureGraphicsDef;

    [ExportGroup("Construction")]
    [Export]
    public ItemDefAmount[] ComponentAmounts;

    [Export]
    public float ConstructionTime = 10f;

    [Export]
    public float DeconstructionTimeMultiplier = .5f;

    [ExportGroup("Navigation")]
    [Export]
    public bool Traversable = true;

    // [ExportGroup("Status")]
    // public bool SetupRequired = false;

    [ExportGroup("Health")]
    [Export]
    public uint MaxHealthPoints = 100;

    // [ExportGroup("Resource processing")]
    // public StateGraphAsset ResourceProcessor;

    public abstract bool IsConstructibleAt(
        EntityGrids entityGrids,
        Vector2I coord,
        bool ignoreExisting = false
    );
}
