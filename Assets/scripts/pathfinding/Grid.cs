using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Terrain terrain;                                                            //refernce to the terrain the grid is on
    public LayerMask unwalkableMask;                                                   //which collision are not walkable
    public Vector2 gridWorldSize;                                                      //size of the grid in world dimensions
    public float nodeRadius;                                                           //the radius of a node
    Node[,] grid;                                                                      //the grid it self

    float nodeDiameter;                                                                //the diameter of a node
    int gridSizeX, gridSizeY;                                                          //the amount of nodes on X and Y of the grid
    float[,] heights;

    //gets the grids size and callculates the size nodes need to be to fill the world size
    private void Start()
    {
        if(terrain != null)
        {
            gridSizeX = terrain.terrainData.heightmapWidth;
            gridSizeY = terrain.terrainData.heightmapHeight;

            nodeDiameter = gridWorldSize.x / gridSizeX;
            nodeRadius = nodeDiameter / 2;
        }
        else
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX =  Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        }
        
        CreateGrid();
    }

    //creates a new node for each position of the grid and checks its collisions
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        if(terrain != null)
            heights = terrain.terrainData.GetHeights(0, 0, gridSizeX, gridSizeY);
      
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                if(terrain != null)
                    worldPoint.y = heights[y,x] * terrain.terrainData.heightmapResolution + 1;
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius,unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint,x,y);
            }
        }
    }

    //gets the clossest node form a world position
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];

    }

    //returns the first walkable node form a world positon
    public Node getSafeNodeFromWordlPoint(Vector3 worldPosition)
    {
        Node actualNode = NodeFromWorldPoint(worldPosition);
        if (actualNode.walkable)
            return actualNode;
        return getSafePointFromNeighbours(actualNode);
    }

    //returns the first walkable neighbour node of the given node
    public Node getSafePointFromNeighbours(Node node)
    {
        List<Node> neighbours = getNeighbours(node);
        foreach (Node n in neighbours)
        {
            if (n.walkable)
                return n;
        }
        foreach (Node n in neighbours)
        {
            return getSafePointFromNeighbours(n);
        }
        return null;
    }

    //returns all the neighbour nodes of the given node
    public List<Node> getNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                int checkX = node.gridx + x;
                int checkY = node.gridy + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >=0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    //rechecks the collisions of each node if the grid
    public void CheckCollisions()
    {
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                n.walkable = !(Physics.CheckSphere(n.worldPosition, nodeRadius, unwalkableMask));
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.DrawCube(n.worldPosition, new Vector3(nodeRadius, nodeRadius, nodeRadius));
            }
        }
       
    }
}
