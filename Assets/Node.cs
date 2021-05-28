using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public int  xIndex , yIndex;
    private Vector3 position;
    private int heapIndex;
    public bool isWalkable;
    public Node parent;
    
    public int gCost;
    public int hCost;
    public int fCost
    {
        get => gCost + hCost;
    }


    public int HeapIndex
    {
        get => heapIndex;
        set => heapIndex = value;
    }

    public Node(bool isWalkable , Vector3 position , int xIndex , int yIndex)
    {
        this.isWalkable = isWalkable;
        this.position = position;
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        parent = null;
    }


    public Vector3 GetWorldPosition()
    {
        return position;
    }

    public int CompareTo(Node node)
    {
        int compareCost = fCost.CompareTo(node.fCost);
        if (compareCost == 0)
            compareCost = hCost.CompareTo(node.hCost);

        return -compareCost;
    }
}

