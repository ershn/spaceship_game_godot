using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class ItemGrid : NodeListGrid
{
    readonly Dictionary<ItemDef, HashSet<Node>> _itemDefIndex = new();
    readonly Dictionary<Type, HashSet<Node>> _itemDefTypeIndex = new();

    public ItemGrid(INodeGrid twinGrid = null)
        : base(twinGrid) { }

    public bool TryGetItem(Vector2I position, ItemDef itemDef, out Node node)
    {
        node = Get(position)
            .FirstOrDefault(item => item.GetNode<ItemDefHolder>("DefHolder").ItemDef == itemDef);
        return node is not null;
    }

    public IEnumerable<Node> Filter(ItemDef itemDef) =>
        _itemDefIndex.TryGetValue(itemDef, out var list) ? list : new();

    public IEnumerable<Node> Filter<T>()
        where T : ItemDef => _itemDefTypeIndex.TryGetValue(typeof(T), out var list) ? list : new();

    public override void Add(Vector2I position, Node node)
    {
        base.Add(position, node);

        var itemDef = node.GetNode<ItemDefHolder>("DefHolder").ItemDef;
        AddToIndex(_itemDefIndex, itemDef, node);
        AddToIndex(_itemDefTypeIndex, itemDef.GetType(), node);
    }

    public override void Remove(Vector2I position, Node node)
    {
        base.Remove(position, node);

        var itemDef = node.GetNode<ItemDefHolder>("DefHolder").ItemDef;
        _itemDefIndex[itemDef].Remove(node);
        _itemDefTypeIndex[itemDef.GetType()].Remove(node);
    }

    void AddToIndex<TKey>(Dictionary<TKey, HashSet<Node>> index, TKey key, Node node)
    {
        if (!index.TryGetValue(key, out var set))
        {
            set = new();
            index[key] = set;
        }
        set.Add(node);
    }
}
