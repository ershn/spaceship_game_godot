using Godot;

public class ArrayGrid<T>
{
    readonly T[,] _array;
    Vector2I _centerPosition;

    public ArrayGrid(uint sideSize)
    {
        _array = new T[sideSize, sideSize];
        _centerPosition = new((int)sideSize / 2, (int)sideSize / 2);
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
