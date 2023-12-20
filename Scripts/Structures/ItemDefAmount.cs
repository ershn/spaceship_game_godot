using Godot;

[GlobalClass]
public partial class ItemDefAmount : Resource
{
    [Export]
    public ItemDef ItemDef;

    [Export(hintString: AmountHint.ModeOf + "ItemDef")]
    public ulong Amount;
}
