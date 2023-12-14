using Godot;

public class ArrayGrid<T>
{
    readonly T[,] _array;
    Vector2I _centerPosition;

    public ArrayGrid(Vector2I size)
    {
        _array = new T[size.X, size.Y];
        _centerPosition = new(size.X / 2, size.Y / 2);
    }

    public T this[Vector2I position]
    {
        get
        {
            position += _centerPosition;
            return _array[position.X, position.Y];
        }
        set
        {
            position += _centerPosition;
            _array[position.X, position.Y] = value;
        }
    }
}
