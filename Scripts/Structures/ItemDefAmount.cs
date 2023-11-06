using Godot;

[GlobalClass]
public partial class ItemDefAmount : Resource
{
    [Export]
    public ItemDef ItemDef;

    // TODO: display as mass/count
    [Export]
    public ulong Amount;

    public AmountMode AmountMode => ItemDef.AmountMode;
}
