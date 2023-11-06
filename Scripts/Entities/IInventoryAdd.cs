public interface IInventoryAdd : IPosition
{
    void Add(ItemDef itemDef, ulong amount);
}
