using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public class NodeListGrid : INodeGrid
{
    readonly ArrayGrid<List<Node>> _grid = new(500);
    readonly INodeGrid _twinGrid;

    public NodeListGrid(INodeGrid twinGrid = null)
    {
        _twinGrid = twinGrid;
    }

    public IEnumerable<Node> Get(Vector2I position) => _grid[position] ?? new();

    public virtual void Add(Vector2I position, Node node)
    {
        var list = _grid[position];
        if (list is null)
        {
            list = new();
            _grid[position] = list;
        }
        list.Add(node);

        _twinGrid?.Add(position, node);
    }

    public virtual void Remove(Vector2I position, Node node)
    {
        var list = _grid[position];
        Debug.Assert(list is not null);

        var removed = list.Remove(node);
        Debug.Assert(removed);

        if (list.Count == 0)
            _grid[position] = null;

        _twinGrid?.Remove(position, node);
    }
}
