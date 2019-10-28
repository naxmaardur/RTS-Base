using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    
    Grid grid;                                                                         //refernce to the grid

    //gets the referce to the grid
    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    //gets the coresponding nodes based of position and then callculates the path
    public List<Node> FindPath(Vector3 start, Vector3 end)
    {
        Node startNode = NodeFromWorldPoint(start);
        Node endNode = NodeFromWorldPoint(end);
        return FindPath(startNode, endNode);
    }

    //callculates a path using A pathfinding
    public List<Node> FindPath(Node startNode, Node endNode)
    {
        if (startNode == null || endNode == null)
            return null;

        List<Node> open = new List<Node>();
        HashSet<Node> closed = new HashSet<Node>();
        open.Add(startNode);
        while (open.Count > 0)
        {
            Node current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if(open[i].fCost < current.fCost || open[i].fCost == current.fCost && open[i].hCost < current.hCost)
                {
                    current = open[i];
                }
            }
            open.Remove(current);
            closed.Add(current);

            if(current == endNode)
            {

                return path(startNode, endNode);
            }

            foreach(Node n in grid.getNeighbours(current))
            {
                if (!n.walkable || closed.Contains(n))
                    continue;

                int newMovementCost = current.gCost + GetDistance(current, n);
                if(newMovementCost < n.gCost || !open.Contains(n))
                {
                    n.gCost = newMovementCost;
                    n.hCost = GetDistance(n, endNode);
                    n.parent = current;

                    if (!open.Contains(n))
                        open.Add(n);
                    
                }
            }
        }
        return new List<Node>();
    }

    //retraces node parends and makes a path list out of it
    List<Node> path(Node start,Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;

        while(current != start)
        {
            path.Add(current);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    //return the distance between two nodes
    public int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.gridx - b.gridx);
        int disty = Mathf.Abs(a.gridy - b.gridy);

        if (distX > disty)
            return 14 * disty + 10 * (distX - disty);
        return 14 * distX + 10 * (disty - distX);
    }
    //returns the first walkable node on the grid at that world position
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        return grid.getSafeNodeFromWordlPoint(worldPosition);
    }
    //returns the node at that world postion walkable or not
    public Node NodeFromWorldPointNotSafe(Vector3 worldPosition)
    {
        return grid.NodeFromWorldPoint(worldPosition);
    }

    //rechecks the collisions of each node if the grid
    public void checkGridCollisions()
    {
        grid.CheckCollisions();
    }
}
