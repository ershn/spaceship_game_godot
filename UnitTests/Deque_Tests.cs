namespace UnitTests;

[TestClass]
public class Deque_Tests
{
    [TestMethod]
    public void Capacity_DequeInitializedWithCapacity_ReturnCapacity()
    {
        var deque = new Deque<int>(8);

        var capacity = deque.Capacity;

        Assert.AreEqual(capacity, 8);
    }

    [TestMethod]
    public void Capacity_DequeInitializedWithEnumerable_ReturnElementCount()
    {
        var deque = new Deque<int>(new[] { 1, 2, 3 });

        var capacity = deque.Capacity;

        Assert.AreEqual(capacity, 3);
    }

    [TestMethod]
    [DataRow(new int[] { }, 0)]
    [DataRow(new int[] { 1 }, 1)]
    [DataRow(new int[] { 1, 2, 3 }, 3)]
    public void Count_ReturnElementCount(int[] elements, int expectedCount)
    {
        var deque = new Deque<int>(elements);

        var count = deque.Count;

        Assert.AreEqual(count, expectedCount);
    }

    [TestMethod]
    public void IsSynchronized_ReturnFalse()
    {
        var deque = new Deque<int>();

        var isSynchronized = deque.IsSynchronized;

        Assert.IsFalse(isSynchronized);
    }

    [TestMethod]
    [DataRow(new int[] { }, 1, false)]
    [DataRow(new int[] { 1, 2, 3 }, 1, true)]
    [DataRow(new int[] { 1, 2, 3 }, 4, false)]
    public void Contains_ReturnIfElementIsPresent(int[] elements, int key, bool expectedResult)
    {
        var deque = new Deque<int>(elements);

        var found = deque.Contains(key);

        Assert.AreEqual(found, expectedResult);
    }

    [TestMethod]
    public void PeekFront_EmptyBuffer_ThrowException()
    {
        var deque = new Deque<int>();

        Assert.ThrowsException<InvalidOperationException>(() => deque.PeekFront());
    }

    [TestMethod]
    public void PeekFront_NonEmptyBuffer_ReturnFrontElementWithoutDequeueing()
    {
        var deque = new Deque<int>(new[] { 1, 2, 3 });

        var element = deque.PeekFront();

        Assert.AreEqual(element, 1);
        CollectionAssert.AreEqual(deque, new int[] { 1, 2, 3 });
    }

    [TestMethod]
    public void TryPeekFront_EmptyBuffer_ReturnFalseAndDefaultValueWithoutDequeueing()
    {
        var deque = new Deque<int>();

        var found = deque.TryPeekFront(out var element);

        Assert.IsFalse(found);
        Assert.AreEqual(element, default);
        CollectionAssert.AreEqual(deque, Array.Empty<int>());
    }

    [TestMethod]
    public void TryPeekFront_NonEmptyBuffer_ReturnTrueAndFrontElementWithoutDequeueing()
    {
        var deque = new Deque<int>(new[] { 1, 2, 3 });

        var found = deque.TryPeekFront(out var element);

        Assert.IsTrue(found);
        Assert.AreEqual(element, 1);
        CollectionAssert.AreEqual(deque, new int[] { 1, 2, 3 });
    }

    [TestMethod]
    public void PeekBack_EmptyBuffer_ThrowException()
    {
        var deque = new Deque<int>();

        Assert.ThrowsException<InvalidOperationException>(() => deque.PeekBack());
    }

    [TestMethod]
    public void PeekBack_NonEmptyBuffer_ReturnBackElementWithoutDequeueing()
    {
        var deque = new Deque<int>(new[] { 1, 2, 3 });

        var element = deque.PeekBack();

        Assert.AreEqual(element, 3);
        CollectionAssert.AreEqual(deque, new int[] { 1, 2, 3 });
    }

    [TestMethod]
    public void TryPeekBack_EmptyBuffer_ReturnFalseAndDefaultValueWithoutDequeueing()
    {
        var deque = new Deque<int>();

        var found = deque.TryPeekBack(out var element);

        Assert.IsFalse(found);
        Assert.AreEqual(element, default);
        CollectionAssert.AreEqual(deque, Array.Empty<int>());
    }

    [TestMethod]
    public void TryPeekBack_NonEmptyBuffer_ReturnTrueAndBackElementWithoutDequeueing()
    {
        var deque = new Deque<int>(new[] { 1, 2, 3 });

        var found = deque.TryPeekBack(out var element);

        Assert.IsTrue(found);
        Assert.AreEqual(element, 3);
        CollectionAssert.AreEqual(deque, new int[] { 1, 2, 3 });
    }

    [TestMethod]
    [DataRow(new int[] { }, 1, new int[] { 1 })]
    [DataRow(new int[] { 1, 2, 3 }, 0, new int[] { 0, 1, 2, 3 })]
    public void EnqueueFront_AddToBufferStart(int[] input, int value, int[] expectedOutput)
    {
        var deque = new Deque<int>(input);

        deque.EnqueueFront(value);

        CollectionAssert.AreEqual(deque, expectedOutput);
    }

    [TestMethod]
    [DataRow(new int[] { }, 1, new int[] { 1 })]
    [DataRow(new int[] { 1, 2, 3 }, 4, new int[] { 1, 2, 3, 4 })]
    public void EnqueueBack_AddToBufferEnd(int[] input, int value, int[] expectedOutput)
    {
        var deque = new Deque<int>(input);

        deque.EnqueueBack(value);

        CollectionAssert.AreEqual(deque, expectedOutput);
    }

    [TestMethod]
    public void DequeueFront_EmptyBuffer_ThrowException()
    {
        var deque = new Deque<int>();

        Assert.ThrowsException<InvalidOperationException>(() => deque.DequeueFront());
    }

    [TestMethod]
    public void DequeueFront_NonEmptyBuffer_DequeueFrontElement()
    {
        var deque = new Deque<int>(new[] { 1, 2, 3 });

        var element = deque.DequeueFront();

        Assert.AreEqual(element, 1);
        CollectionAssert.AreEqual(deque, new int[] { 2, 3 });
    }

    [TestMethod]
    public void TryDequeueFront_EmptyBuffer_ReturnFalseAndDefaultValueWithoutDequeueing()
    {
        var deque = new Deque<int>();

        var found = deque.TryDequeueFront(out var element);

        Assert.IsFalse(found);
        Assert.AreEqual(element, default);
        CollectionAssert.AreEqual(deque, Array.Empty<int>());
    }

    [TestMethod]
    public void TryDequeueFront_NonEmptyBuffer_ReturnTrueAndDequeueFrontElement()
    {
        var deque = new Deque<int>(new[] { 1, 2, 3 });

        var found = deque.TryDequeueFront(out var element);

        Assert.IsTrue(found);
        Assert.AreEqual(element, 1);
        CollectionAssert.AreEqual(deque, new int[] { 2, 3 });
    }

    [TestMethod]
    public void DequeueBack_EmptyBuffer_ThrowException()
    {
        var deque = new Deque<int>();

        Assert.ThrowsException<InvalidOperationException>(() => deque.DequeueBack());
    }

    [TestMethod]
    public void DequeueBack_NonEmptyBuffer_DequeueBackElement()
    {
        var deque = new Deque<int>(new[] { 1, 2, 3 });

        var element = deque.DequeueBack();

        Assert.AreEqual(element, 3);
        CollectionAssert.AreEqual(deque, new int[] { 1, 2 });
    }

    [TestMethod]
    public void TryDequeueBack_EmptyBuffer_ReturnFalseAndDefaultValueWithoutDequeueing()
    {
        var deque = new Deque<int>();

        var found = deque.TryDequeueBack(out var element);

        Assert.IsFalse(found);
        Assert.AreEqual(element, default);
        CollectionAssert.AreEqual(deque, Array.Empty<int>());
    }

    [TestMethod]
    public void TryDequeueBack_NonEmptyBuffer_ReturnTrueAndDequeueBackElement()
    {
        var deque = new Deque<int>(new[] { 1, 2, 3 });

        var found = deque.TryDequeueBack(out var element);

        Assert.IsTrue(found);
        Assert.AreEqual(element, 3);
        CollectionAssert.AreEqual(deque, new int[] { 1, 2 });
    }

    [TestMethod]
    [DataRow(new int[] { })]
    [DataRow(new int[] { 1, 2, 3 })]
    public void Clear_RemoveAllElements(int[] elements)
    {
        var deque = new Deque<int>(elements);

        deque.Clear();

        Assert.AreEqual(deque.Count, 0);
        CollectionAssert.AreEqual(deque, Array.Empty<int>());
    }

    [TestMethod]
    [DataRow(new int[] { })]
    [DataRow(new int[] { 1, 2, 3 })]
    public void GetEnumerator_ReturnAllElementsInOrder(int[] elements)
    {
        var deque = new Deque<int>(elements);

        var array = deque.ToArray();

        CollectionAssert.AreEqual(array, elements);
    }

    #region ring buffer tests

    [TestMethod]
    public void EnqueueBackDequeueFrontRepetition_CapacityDoesNotIncrease()
    {
        var deque = new Deque<int>(4);

        for (int n = 0; n < 100; n++)
        {
            deque.EnqueueBack(1);
            deque.DequeueFront();
        }

        Assert.AreEqual(deque.Capacity, 4);
    }

    [TestMethod]
    public void EnqueueFrontDequeueBackRepetition_CapacityDoesNotIncrease()
    {
        var deque = new Deque<int>(4);

        for (int n = 0; n < 100; n++)
        {
            deque.EnqueueFront(1);
            deque.DequeueBack();
        }

        Assert.AreEqual(deque.Capacity, 4);
    }

    #endregion
}
