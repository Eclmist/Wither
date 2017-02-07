using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node  {

    public bool isWalkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    private int gCost;
    private int hCost;
    public Node parentNode;

    public Node(bool isWalkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.isWalkable = isWalkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int F_Cost
    {
        get { return gCost + hCost; }
    }

    public int H_Cost
    {
        get { return this.hCost; }
        set { this.hCost = value; }
    }

    public int G_Cost
    {
        get { return this.gCost; }
        set { this.gCost = value; }
    }
}
