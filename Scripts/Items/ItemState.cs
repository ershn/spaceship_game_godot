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
}
