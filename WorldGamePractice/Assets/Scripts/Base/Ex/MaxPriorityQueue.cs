using System;
using System.Collections.Generic;

/// <summary>
/// 泛型大顶堆优先队列（Max-Heap Priority Queue）
/// 元素越大（按比较器定义），优先级越高，越先出队。
/// </summary>
/// <typeparam name="T">队列中元素的类型</typeparam>
public class MaxPriorityQueue<T>
{
    private readonly List<T> _heap;
    private readonly Comparison<T> _compare;

    #region Constructors

    /// <summary>
    /// 使用默认比较器（要求 T 实现 IComparable<T> 或有默认 Comparer）
    /// </summary>
    public MaxPriorityQueue()
        : this(Comparer<T>.Default)
    {
    }

    /// <summary>
    /// 使用自定义 IComparer<T>
    /// </summary>
    public MaxPriorityQueue(IComparer<T> comparer)
    {
        if (comparer == null)
            throw new ArgumentNullException(nameof(comparer));
        _heap = new List<T>();
        _compare = comparer.Compare;
    }

    /// <summary>
    /// 使用自定义 Comparison<T> 委托
    /// </summary>
    public MaxPriorityQueue(Comparison<T> comparison)
    {
        if (comparison == null)
            throw new ArgumentNullException(nameof(comparison));
        _heap = new List<T>();
        _compare = comparison;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// 获取队列中元素的数量
    /// </summary>
    public int Count => _heap.Count;

    /// <summary>
    /// 判断队列是否为空
    /// </summary>
    public bool IsEmpty => _heap.Count == 0;

    #endregion

    #region Public Methods

    /// <summary>
    /// 将元素加入优先队列
    /// 时间复杂度: O(log n)
    /// </summary>
    public void Enqueue(T item)
    {
        _heap.Add(item);
        HeapifyUp(_heap.Count - 1);
    }

    /// <summary>
    /// 移除并返回优先级最高的元素（最大值）
    /// 时间复杂度: O(log n)
    /// </summary>
    /// <exception cref="InvalidOperationException">当队列为空时抛出</exception>
    public T Dequeue()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Cannot dequeue from an empty priority queue.");

        T root = _heap[0];
        int lastIdx = _heap.Count - 1;
        _heap[0] = _heap[lastIdx];
        _heap.RemoveAt(lastIdx);

        if (_heap.Count > 0)
            HeapifyDown(0);

        return root;
    }

    /// <summary>
    /// 返回优先级最高的元素，但不移除
    /// 时间复杂度: O(1)
    /// </summary>
    /// <exception cref="InvalidOperationException">当队列为空时抛出</exception>
    public T Peek()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Cannot peek an empty priority queue.");
        return _heap[0];
    }

    /// <summary>
    /// 清空队列
    /// </summary>
    public void Clear()
    {
        _heap.Clear();
    }

    #endregion

    #region Private Helper Methods

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) >> 1; // (index - 1) / 2
            if (_compare(_heap[index], _heap[parentIndex]) <= 0)
                break; // 满足堆性质：父 >= 子

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        int count = _heap.Count;
        while (true)
        {
            int left = (index << 1) + 1;   // 2 * index + 1
            int right = left + 1;          // 2 * index + 2
            int largest = index;

            if (left < count && _compare(_heap[left], _heap[largest]) > 0)
                largest = left;

            if (right < count && _compare(_heap[right], _heap[largest]) > 0)
                largest = right;

            if (largest == index)
                break;

            Swap(index, largest);
            index = largest;
        }
    }

    private void Swap(int i, int j)
    {
        T temp = _heap[i];
        _heap[i] = _heap[j];
        _heap[j] = temp;
    }

    #endregion
}