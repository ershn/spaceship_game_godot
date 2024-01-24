using Godot;

[Tool, GlobalClass]
public partial class ItemDefAmount : Resource
{
    [Export]
    public ItemDef ItemDef;

    [Export(hintString: AmountHint.ModeOf + nameof(ItemDef))]
    public ulong Amount;
}
