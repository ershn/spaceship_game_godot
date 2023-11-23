using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Deque<T> : IEnumerable<T>, ICollection
{
    const int _defaultCapacity = 8;

    T[] _buffer;
    int _count;
    int _frontIndex;
    int _backIndex;

    public Deque(int capacity = _defaultCapacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _buffer = new T[capacity];
    }

    public Deque(IEnumerable<T> values)
    {
        _buffer = values.ToArray();
        _count = _buffer.Length;
        _backIndex = Math.Max(0, _count - 1);
    }

    public int Capacity => _buffer.Length;

    public int Count => _count;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public bool Contains(T element)
    {
        foreach (var arrayElement in this)
        {
            if (EqualityComparer<T>.Default.Equals(arrayElement, element))
                return true;
        }
        return false;
    }

    public T PeekFront() => Peek(_frontIndex);

    public bool TryPeekFront(out T element) => TryPeek(_frontIndex, out element);

    public T PeekBack() => Peek(_backIndex);

    public bool TryPeekBack(out T element) => TryPeek(_backIndex, out element);

    T Peek(int index)
    {
        if (_count == 0)
            throw new InvalidOperationException("The deque is empty");

        return _buffer[index];
    }

    bool TryPeek(int index, out T element)
    {
        if (_count == 0)
        {
            element = default;
            return false;
        }
        else
        {
            element = Peek(index);
            return true;
        }
    }

    public void EnqueueFront(T element)
    {
        EnsureSpaceIsAvailable();
        if (_count > 0)
            _frontIndex = OffsetIndex(_frontIndex, -1);
        _buffer[_frontIndex] = element;
        _count++;
    }

    public void EnqueueBack(T element)
    {
        EnsureSpaceIsAvailable();
        if (_count > 0)
            _backIndex = OffsetIndex(_backIndex, +1);
        _buffer[_backIndex] = element;
        _count++;
    }

    public T DequeueFront()
    {
        if (_count == 0)
            throw new InvalidOperationException("The deque is empty");

        var element = _buffer[_frontIndex];
        _buffer[_frontIndex] = default;
        _count--;
        if (_count > 0)
            _frontIndex = OffsetIndex(_frontIndex, +1);
        return element;
    }

    public bool TryDequeueFront(out T element)
    {
        if (_count == 0)
        {
            element = default;
            return false;
        }
        element = DequeueFront();
        return true;
    }

    public T DequeueBack()
    {
        if (_count == 0)
            throw new InvalidOperationException("The deque is empty");

        var element = _buffer[_backIndex];
        _buffer[_backIndex] = default;
        _count--;
        if (_count > 0)
            _backIndex = OffsetIndex(_backIndex, -1);
        return element;
    }

    public bool TryDequeueBack(out T element)
    {
        if (_count == 0)
        {
            element = default;
            return false;
        }
        element = DequeueBack();
        return true;
    }

    public void Clear()
    {
        for (int offset = 0; offset < _count; offset++)
            _buffer[OffsetIndex(_frontIndex, offset)] = default;

        _count = 0;
        _frontIndex = 0;
        _backIndex = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int offset = 0; offset < _count; offset++)
            yield return _buffer[OffsetIndex(_frontIndex, offset)];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void CopyTo(Array array, int index)
    {
        if (_backIndex >= _frontIndex)
            Array.Copy(_buffer, _frontIndex, array, index, _count);
        else
        {
            var countToArrayEnd = _buffer.Length - _frontIndex;
            Array.Copy(_buffer, _frontIndex, array, index, countToArrayEnd);
            Array.Copy(_buffer, 0, array, index + countToArrayEnd, _count - countToArrayEnd);
        }
    }

    int OffsetIndex(int index, int offset)
    {
        index += offset;
        if (index >= _buffer.Length)
            index -= _buffer.Length;
        else if (index < 0)
            index += _buffer.Length;
        return index;
    }

    void EnsureSpaceIsAvailable()
    {
        if (_count < _buffer.Length)
            return;

        int newCapacity;
        if (_buffer.Length == 0)
            newCapacity = 1;
        else if (BitOperations.IsPow2(_buffer.Length))
            newCapacity = _buffer.Length * 2;
        else
            newCapacity = (int)BitOperations.RoundUpToPowerOf2((uint)_buffer.Length);

        var newBuffer = new T[newCapacity];

        int index = 0;
        foreach (var bufferElement in this)
            newBuffer[index++] = bufferElement;

        _buffer = newBuffer;
        _frontIndex = 0;
        _backIndex = Math.Max(0, _count - 1);
    }
}
