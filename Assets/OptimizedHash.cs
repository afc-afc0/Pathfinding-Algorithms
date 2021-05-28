using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizedHash 
{
    int itemCount = 0;
    int size;
    int currentMinIndex;
    MinHeap<Node>[] hash;


    public OptimizedHash(int size)
    {
        this.size = size;
        hash = new MinHeap<Node>[size];
        for (int i = 0; i < size; i++)
            hash[i] = new MinHeap<Node>(size / 15);
    }

    public void Insert(Node node)
    {
        if (currentMinIndex > node.fCost)
            currentMinIndex = node.fCost / 5;

        hash[currentMinIndex].Add(node);
        itemCount++;
    }

    public Node RemoveMin()
    {
        Node minNode = hash[currentMinIndex].RemoveFirst();

        UpdateCurrentMinIndex();

        itemCount--;
        return minNode;
    }

    private void UpdateCurrentMinIndex()
    {

        while (hash[currentMinIndex++].Count != 0)
        {
            Debug.Log("Working");
            break;
        }
    }

    public bool Contains(Node node)
    {
        return hash[node.fCost / 5].Contains(node);
    }

    public void UpdateItem(Node node)
    {
        hash[node.fCost / 5].UpdateItem(node);
    }

    public int Count
    {
        get => itemCount;
    }

}
