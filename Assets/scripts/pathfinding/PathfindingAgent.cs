using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingAgent : Base
{
    protected Vector3 targetPosition;                                                  //the position in world space to pathfind to

    protected Node targetNode;                                                         //the node the targetposition is at
    protected Node selfNode;                                                           //the node the agent is at
    List<Node> path;                                                                   //list of nodes to follow

    float moveSpeed = 0.1f;                                                            //the speed the agent will move at
    float rotationSpeed = 0.1f;                                                        //the speed the agent will rotate at
    float stopDistanse;                                                                //the distance from targetposition to stop moving
    protected bool followPath = true;                                                  //if the agent should follow the path

    // Start is called before the first frame update
    //making sure targetposition is not 0,0,0
    protected virtual void Start()
    {
        if(targetPosition == Vector3.zero)
            targetPosition = transform.position;
    }

    // Update is called once per frame
    //gets the current node the target and agent is if the target is at a new node recalculate path
    //starts followPath function
    protected void Update()
    {
        Node newNode = Global.pathfinding.NodeFromWorldPoint(targetPosition);
        Node newSelfNode = Global.pathfinding.NodeFromWorldPoint(transform.position);
        if (targetNode != newNode)
        {
           
            targetNode = newNode;
            selfNode = newSelfNode;
            path = Global.pathfinding.FindPath(selfNode, targetNode);
        }

        FollowPath();

    }

    //moves the agent to the first node of the path and then removes it
    void FollowPath()
    {
        if (path != null && followPath)
        {

            if (path.Count > 0)
            {
                if (!path[0].walkable)
                    targetNode = null;
                if (path.Count == 1 & Global.pathfinding.NodeFromWorldPointNotSafe(targetPosition).walkable)
                {
                    Vector3 pos = targetPosition;
                    pos.y = targetNode.worldPosition.y;
                    MoveTo(pos);
                }
                    
                else
                    MoveTo(path[0].worldPosition);

               
            }
            else
            EndOffPath();
        }
    }
    //gets called at the end of a path
   protected virtual void EndOffPath()
    {

    }
    //moves the agent to a position in the world
    void MoveTo(Vector3 pos)
    {
        transform.position = Vector3.MoveTowards(transform.position, pos, moveSpeed);
        transform.LookAt(pos);
        if (Vector3.Distance(pos, transform.position) <= 0)
            path.Remove(path[0]);
    }
    //draws gizmos of the current paths
    private void OnDrawGizmos()
    {
        if (path != null)
        {
            if (path.Count > 0)
            {
                foreach (Node n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (1 - .1f));
                }
            }
        }
    }
}
