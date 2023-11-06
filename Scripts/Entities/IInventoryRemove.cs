public interface IInventoryRemove : IPosition
{
    void Remove(ItemDef itemDef, ulong amount);
}
