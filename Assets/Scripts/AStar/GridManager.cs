using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

    public Vector2 gridSize;
    public float nodeRadius;
    public LayerMask obstacleLayerMask;
    public static GridManager instance;

    // 2D array of nodes
    Node[,] grid;
    float nodeDiameter;
    int nrOfNodesX; // Number of nodes along the X axis of the grid
    int nrOfNodesY; // Number of nodes along the Z axis of the grid

    void Awake()
    {
        instance = this;
        nodeDiameter = nodeRadius * 2;
        // half a node doesn't exist!
        nrOfNodesX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        nrOfNodesY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid();
    }

    // Called when obstacles are removed
    public void RecalculateObstacles()
    {
        StartCoroutine("Recalculate");
    }

    IEnumerator Recalculate()
    {
        foreach (Node n in grid)
        {
            n.isWalkable = !(Physics.CheckSphere(n.worldPosition, nodeRadius, obstacleLayerMask));
        }

        yield return null;

    }

    void CreateGrid()
    {
        grid = new Node[nrOfNodesX, nrOfNodesY];

        // The bottom left corner of the grid
        Vector3 bottomLeft = transform.position - Vector3.right * gridSize.x/2 - Vector3.forward * gridSize.y/2;

        // Loop through every node
        for(int i = 0; i<nrOfNodesX; i++)
        {
            for (int j = 0; j < nrOfNodesY; j++)
            {
                // Assign a world position to the node
                Vector3 worldPoint = bottomLeft + Vector3.right * (i * nodeDiameter + nodeRadius) + Vector3.forward * (j * nodeDiameter + nodeRadius);
                // Check if there are obstacles on the node
                bool isWalkable = !(Physics.CheckSphere(worldPoint, nodeRadius,obstacleLayerMask));
                // Create the Node
                grid[i, j] = new Node(isWalkable,worldPoint,i,j);
            }
        }
    }

    public Node GetNodeInGrid(Vector3 worldPosition)
    {
        // Calculate percentage of distance relative to the grid
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentY= (worldPosition.z + gridSize.y / 2) / gridSize.y;
        //prevent errors when worldPosition is out of the grid
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //Get the index of the node in the grid
        int x = Mathf.RoundToInt((nrOfNodesX - 1) * percentX);
        int y = Mathf.RoundToInt((nrOfNodesY - 1) * percentY);

        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // get all the nodes surrounding it
        for(int i = -1; i <=1; i++)
        {
            for(int j = -1; j <=1; j++)
            {
                // Skip initial node
                if (i == 0 && j == 0)
                    continue;

                int x = node.gridX + i;
                int y = node.gridY + j;

                // Ensure neighbouring node is within the grid
                if (x >= 0 && x < nrOfNodesX && y >= 0 && y < nrOfNodesY)
                    neighbours.Add(grid[x,y]);

            }
        }

        return neighbours;
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position,new Vector3(gridSize.x,1,gridSize.y));

        if(grid != null)
        {
            foreach( Node n in grid)
            {

                if (n.isWalkable)
                    Gizmos.color = Color.white;
                else
                    Gizmos.color = Color.red;

               

                Gizmos.DrawCube(n.worldPosition,Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

}
