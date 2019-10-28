using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gCost;
    public int hCost;

    public int gridx;
    public int gridy;

    public Node parent;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }

    }


    public Node(bool walkable,Vector3 worldPosition,int x, int y)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        gridx = x;
        gridy = y;
    }

   
}
