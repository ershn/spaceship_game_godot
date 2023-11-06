using Godot;

public interface INodeGrid
{
    void Add(Vector2I position, Node node);
    void Remove(Vector2I position, Node node);
}
