using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFinder : MonoBehaviour {

    // Store a reference to grid
    GridManager grid;
    
    void Awake()
    {
        grid = GetComponent<GridManager>();
    }

    public void StartFindingPath(Vector3 startPos, Vector3 endPos)
    {
        StartCoroutine(FindPath(startPos,endPos));
    }

    int CalculateDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        else
            return 14 * distX + 10 * (distY - distX);
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeInGrid(startPos);
        Node targetNode = grid.GetNodeInGrid(targetPos);

        Vector3[] wayPoints = new Vector3[0];
        bool pathFound = false;

        // Check if search is possible 
        if (startNode.isWalkable && targetNode.isWalkable)
        {

            // Create lists to store open and closed nodes
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();
            openList.Add(startNode);

            while (openList.Count > 0)
            {

                Node currentNode = openList[0];

                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].F_Cost < currentNode.F_Cost || openList[i].F_Cost == currentNode.F_Cost && openList[i].H_Cost < currentNode.H_Cost)
                        currentNode = openList[i];
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                // If target node has been found
                if (currentNode == targetNode)
                {
                    pathFound = true;
                    break;
                }


                foreach (Node n in grid.GetNeighbours(currentNode))
                {
                    // Skip neighbour nodes that are unwalkable or are in the closed list
                    if (!n.isWalkable || closedList.Contains(n))
                        continue;

                    int newCostToNeighbour = currentNode.G_Cost + CalculateDistance(currentNode, n);

                    // Update the new F cost of the neighbour node
                    if (newCostToNeighbour < n.G_Cost || !openList.Contains(n))
                    {
                        n.G_Cost = newCostToNeighbour;
                        n.H_Cost = CalculateDistance(n, targetNode);
                        n.parentNode = currentNode;

                        // Add the neighbour node if not in open list
                        if (!openList.Contains(n))
                            openList.Add(n);

                    }
                }
            }
        }

        yield return null;

        if(pathFound)
        {
            wayPoints = RetracePath(startNode, targetNode);
        }
        PathRequestManager.instance.FinishedProcessingPath(wayPoints,pathFound);
        
    }


    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        Vector3[] wayPoints = SimplifyPath(path);
        Array.Reverse(wayPoints);
        return wayPoints;
        
    }



    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> wayPoints = new List<Vector3>();

        // Store the direction of the last 2 nodes
        Vector2 oldDirection = Vector2.zero;

        // WEIRD
        //wayPoints.Add(path[0].worldPosition);

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 newDirection = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);

            // Path has changed directions
            if(newDirection != oldDirection)
            {
                wayPoints.Add(path[i-1].worldPosition);
            }

            oldDirection = newDirection;

        }

        return wayPoints.ToArray();
    }

	
}
