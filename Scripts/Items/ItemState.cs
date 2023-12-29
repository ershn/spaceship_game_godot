using Godot;

[Tool]
public partial class ItemState : EntityState
{
    public override EntityDef EntityDef => ItemDef;

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

    public override void Initialize(Node2D item)
    {
        item.GetNode<ItemAmount>("ItemAmount").Initialize(Amount);
    }

    public override void EditorInitialize(Node2D item)
    {
        ItemAmount.EditorInitialize(item.GetNode("ItemAmount"), Amount);
    }

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        if ((string)property["name"] == nameof(ItemDef))
            property["usage"] = (long)(PropertyUsageFlags.ReadOnly | PropertyUsageFlags.Storage);
    }
}
