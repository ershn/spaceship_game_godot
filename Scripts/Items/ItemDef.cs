using System.Linq;
using Godot;

[Tool, GlobalClass]
public partial class ItemDef : EntityDef, IWorldLayerGet, IAmountModeGet
{
    public override ItemState NewState() => new(this);

    public WorldLayer WorldLayer => WorldLayer.Item;

    [ExportGroup("Amount")]
    [Export]
    AmountMode _amountMode;
    public virtual AmountMode AmountMode => _amountMode;

    [ExportGroup("Graphics")]
    public override Texture2D PreviewSprite => AmountSprites?.FirstOrDefault()?.Sprite;

    [Export]
    public AmountSprite[] AmountSprites;
}
