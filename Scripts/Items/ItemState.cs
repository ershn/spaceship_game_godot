using Godot;

[Tool]
public partial class ItemState : Resource
{
    [Export]
    public ItemDef ItemDef;

    [Export(hintString: AmountHint.ModeOf + "ItemDef")]
    public ulong Amount;

    ItemState() { }

    public ItemState(ItemDef itemDef)
    {
        ItemDef = itemDef;
        Amount = itemDef.AmountMode.DefaultAmount;
    }

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        if ((string)property["name"] == nameof(ItemDef))
            property["usage"] = (long)PropertyUsageFlags.ReadOnly;
    }
}
