using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public MinHeap(int maxSize)
    {
        items = new T[maxSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T result = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);

        return result;
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex] , item);
    }

    public int Count
    {
        get => currentItemCount;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    private void SortDown(T item)
    {
        while(true)
        {
            int childLeft = item.HeapIndex * 2 + 1;
            int childRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childLeft < currentItemCount)
            {
                swapIndex = childLeft;

                if (childRight < currentItemCount)
                    if (items[childLeft].CompareTo(items[childRight]) < 0)
                        swapIndex = childRight;

                if (item.CompareTo(items[swapIndex]) < 0)
                    Swap(item, items[swapIndex]);
                else
                    return;
            }
            else
                return;
        }
    }

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while(true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
                break;

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    private void Swap(T first , T second)
    {
        items[first.HeapIndex] = second;
        items[second.HeapIndex] = first;
        int tmp = first.HeapIndex;
        first.HeapIndex = second.HeapIndex;
        second.HeapIndex = tmp;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
