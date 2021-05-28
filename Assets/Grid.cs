using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Transform agentTransform;
    [SerializeField] private Vector2 gridWorldSize;
    [SerializeField] private float nodeRadius;
    [SerializeField] private LayerMask unwalkableMask;
    private int xSize, ySize;
    private float nodeDiameter;

    Node[,] grid;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;

        xSize = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        ySize = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        Debug.Log("x size = " + xSize + " y size = " + ySize);

        GenerateGrid();
    }

    public int MaxSize
    {
        get => xSize * ySize;
    }

    Vector3 worldBottomLeft;
    private void GenerateGrid()
    {
        grid = new Node[xSize, ySize];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for(int i = 0;i < xSize;i++)
        {
            for(int j = 0;j < ySize;j++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (i * nodeDiameter + nodeRadius) + Vector3.forward * (j * nodeDiameter + nodeRadius);
                //bool walkable = !(Physics.CheckSphere(worldPoint , nodeDiameter ,unwalkableMask));
                bool walkable = !(Physics.CheckCapsule(worldPoint , worldPoint + Vector3.up * 5, 0.1f , unwalkableMask));
                grid[i,j] = new Node(walkable , worldPoint , i , j);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> result = new List<Node>();
        int currentX = node.xIndex, currentY = node.yIndex;

        if (currentX >= 0 && currentY - 1 >= 0) result.Add(grid[currentX, currentY - 1]);
        if (currentX - 1 >= 0 && currentY >= 0) result.Add(grid[currentX - 1, currentY]);
        if (currentX + 1 < xSize && currentY < ySize) result.Add(grid[currentX + 1, currentY]);
        if (currentX < xSize && currentY + 1 < ySize) result.Add(grid[currentX, currentY + 1]);
        if (currentX + 1 < xSize && currentY + 1 < ySize) result.Add(grid[currentX + 1, currentY + 1]);
        if (currentX - 1 >= 0 && currentY + 1 < ySize) result.Add(grid[currentX - 1, currentY + 1]);
        if (currentX + 1 < xSize && currentY - 1 >= 0) result.Add(grid[currentX + 1, currentY - 1]);
        if (currentX - 1 >= 0 && currentY - 1 >= 0) result.Add(grid[currentX - 1, currentY - 1]);

        return result;
    }


    public Node GetNodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int xPos = Mathf.RoundToInt((xSize - 1) * percentX);
        int yPos = Mathf.RoundToInt((ySize - 1) * percentY);

        return grid[xPos, yPos];
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position , new Vector3(gridWorldSize.x , 1 , gridWorldSize.y));

        if(grid != null)
        {
            Node playerNode = GetNodeFromWorldPoint(agentTransform.position);
            for(int i = 0;i < xSize;i++)
            {
                for(int j = 0;j < ySize;j++)
                {
                    Gizmos.color = grid[i, j].isWalkable ? Color.white : Color.red;
                    if (grid[i, j] == playerNode) Gizmos.color = Color.cyan;
                    Gizmos.DrawCube(grid[i,j].GetWorldPosition() , Vector3.one * (nodeDiameter-.1f));
                }
            }
        }
    }



}
