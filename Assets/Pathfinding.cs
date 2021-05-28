using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    #region SINGLETON
    private static Pathfinding instance;
    public static Pathfinding Instance
    {
        get => instance;
    }
    #endregion

    [SerializeField] private Transform agent;
    [SerializeField] private Transform target;
    Grid grid;

    private void Awake()
    {
        instance = this;
        grid = GetComponent<Grid>();
    }

    public void FindPath(PathRequest request , Action<PathResult> callBack)
    {
        Debug.Log("With Min Heap");
        Vector3[] result = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.GetNodeFromWorldPoint(request.pathStart);
        Node targetNode = grid.GetNodeFromWorldPoint(request.pathEnd);

        if (startNode.isWalkable && targetNode.isWalkable)
        {

            MinHeap<Node> openSet = new MinHeap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (closedSet.Contains(neighbour) || !(neighbour.isWalkable))
                        continue;

                    int movementCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (movementCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = movementCost;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }


            if (pathSuccess)
                result = RetracePath(startNode, targetNode);

            callBack(new PathResult(result , pathSuccess ,request.callback));
        }
    }

    public void FindPath2(PathRequest request, Action<PathResult> callBack)
    {
        Debug.Log("With Optimized Hash");
        Vector3[] result = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.GetNodeFromWorldPoint(request.pathStart);
        Node targetNode = grid.GetNodeFromWorldPoint(request.pathEnd);

        if (startNode.isWalkable && targetNode.isWalkable)
        {

            OptimizedHash optimizedHash = new OptimizedHash(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            optimizedHash.Insert(startNode);

            while (optimizedHash.Count > 0)
            {
                Node currentNode = optimizedHash.RemoveMin();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (closedSet.Contains(neighbour) || !(neighbour.isWalkable))
                        continue;

                    int movementCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (movementCost < neighbour.gCost || !optimizedHash.Contains(neighbour))
                    {
                        neighbour.gCost = movementCost;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!optimizedHash.Contains(neighbour))
                            optimizedHash.Insert(neighbour);
                        else
                            optimizedHash.UpdateItem(neighbour);
                    }
                }
            }


            if (pathSuccess)
                result = RetracePath(startNode, targetNode);

            callBack(new PathResult(result, pathSuccess, request.callback));
        }
    }

    /*private Vector3[] RetracePath(Node startNode , Node endNode)
    {
        //List<Node> result = new List<Node>();
        int xDifference = Mathf.Abs(startNode.xIndex - endNode.xIndex);
        int yDifference = Mathf.Abs(startNode.xIndex - endNode.yIndex);

        Node[] result = new Node[xDifference + yDifference];

        Node currentNode = startNode;

        int i = 0;
        while (currentNode != endNode)
        {
            Debug.Log("i = " + i);
            result[i] = currentNode;
            currentNode = currentNode.parent;
            i++;
        }

        return SimplifyPath(result);
    }

    private Vector3[] SimplifyPath(Node[] nodes)
    {
        List<Vector3> result = new List<Vector3>();
        Vector2 oldDirection = Vector2.zero;

        for(int i = 1;i < nodes.Length;i++)
        {
            Vector2 newDirection = new Vector2(nodes[i - 1].xIndex - nodes[i].xIndex,nodes[i - 1].yIndex - nodes[i].yIndex);
            if (newDirection != oldDirection)
                result.Add(nodes[i].GetWorldPosition());

            oldDirection = newDirection;
        }

        result.Reverse();

        return result.ToArray();
    }*/

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        waypoints.Add(target.position);

        for (int i = 1; i < path.Count; i++)
            waypoints.Add(path[i].GetWorldPosition());

        /*for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].xIndex - path[i].xIndex, path[i - 1].yIndex - path[i].yIndex);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].GetWorldPosition());
            }
            directionOld = directionNew;
        }*/
        return waypoints.ToArray();
    }

    Vector3[] SimplifyPath2(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        waypoints.Add(target.position);

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].xIndex - path[i].xIndex, path[i - 1].yIndex - path[i].yIndex);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].GetWorldPosition());
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    private int GetDistance(Node a,Node b)
    {
        int distanceX = Mathf.Abs(a.xIndex - b.xIndex);
        int distanceY = Mathf.Abs(a.yIndex - b.yIndex);

        return (distanceX > distanceY) ? 15 * distanceY + 10 *(distanceX - distanceY) : 15 * distanceX + 10 * (distanceY - distanceX);
    }

}
