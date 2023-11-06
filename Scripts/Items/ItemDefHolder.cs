using Godot;

[GlobalClass]
public partial class ItemDefHolder : EntityDefHolder, ITemplate<ItemDef>, IWorldLayerGet
{
    public void Template(ItemDef itemDef)
    {
        ItemDef = itemDef;
    }

    public override EntityDef EntityDef => ItemDef;

    [Export]
    public ItemDef ItemDef;

    public WorldLayer WorldLayer => ItemDef.WorldLayer;
}
