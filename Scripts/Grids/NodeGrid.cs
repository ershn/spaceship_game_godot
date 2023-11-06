using System.Diagnostics;
using Godot;

public class NodeGrid : INodeGrid
{
    readonly ArrayGrid<Node> _grid = new(500);
    readonly INodeGrid _twinGrid;

    public NodeGrid(INodeGrid twinGrid = null)
    {
        _twinGrid = twinGrid;
    }

    public Node Get(Vector2I position)
    {
        var node = _grid[position];
        Debug.Assert(node is not null);

        return node;
    }

    public bool TryGet(Vector2I position, out Node node)
    {
        node = _grid[position];
        return node is not null;
    }

    public bool Has(Vector2I position) => _grid[position] is not null;

    public virtual void Add(Vector2I position, Node node)
    {
        Debug.Assert(_grid[position] is null);

        _grid[position] = node;

        _twinGrid?.Add(position, node);
    }

    public virtual void Remove(Vector2I position, Node node)
    {
        Debug.Assert(_grid[position] is not null);

        _grid[position] = null;

        _twinGrid?.Remove(position, node);
    }
}
