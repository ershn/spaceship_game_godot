using Godot;

[GlobalClass]
public partial class ItemDef : EntityDef, IWorldLayerGet
{
    public WorldLayer WorldLayer => WorldLayer.Item;

    [ExportGroup("Amount")]
    [Export]
    AmountMode _amountMode;
    public virtual AmountMode AmountMode => _amountMode;

    [ExportGroup("Graphics")]
    [Export]
    public AmountSprite[] AmountSprites;
}
