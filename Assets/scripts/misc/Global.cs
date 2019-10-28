using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Global
{
    public static Camera cam;                                                          //refernce to the main camera
    public static GameMaster gm;                                                       //refernce to the game master
    public static UIManager uiM;                                                       //refernce to the ui manager
    public static Pathfinding pathfinding;                                             //refernce to the pathfinder
    public static LayerMask groundLayer;                                               //the layer the ground is
    public static int gridSize = 1;                                                    //the size if the grid
    public static List<BuildingObject> buildings = new List<BuildingObject> { };       //list of all buildings
    public static List<UnitObject> units = new List<UnitObject> { };                   //list of all units

    //getting all refernces
    [RuntimeInitializeOnLoadMethod]
    static void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        pathfinding = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<Pathfinding>();
        uiM = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        groundLayer = (1 <<  LayerMask.NameToLayer("Ground"));
        GetBuildings();
        GetUnits();
    }

    //getting all buildingObjects from the resource folder 
    static void GetBuildings()
    {
        Object[] objects = Resources.LoadAll("Buildings", typeof(BuildingObject));
        foreach (Object o in objects)
        {
            BuildingObject b = (BuildingObject)o;
            buildings.Add(b);
        }
    }
    //getting all unitObject from the resource folder 
    static void GetUnits()
    {
        Object[] objects = Resources.LoadAll("Units", typeof(UnitObject));
        foreach (Object o in objects)
        {
            UnitObject b = (UnitObject)o;
            units.Add(b);
        }
    }
    
    //returns the current position of the mouse in world space
    public static Vector3 getPosition()
    {
        var v3 = Input.mousePosition;
        v3.z = 68;
        v3 = cam.ScreenToWorldPoint(v3);
        return v3;
    }

    //return the direction from one point to another
    public static Vector3 getDirection(Vector3 from, Vector3 to)
    {
        Vector3 v = to - from;
        return v.normalized;
    }

    //returns the point of the map the mouse is over
    public static Vector3 getPointOnMap()
    {
        RaycastHit hit;
        Vector3 v3 = new Vector3();
        if (Physics.Raycast(cam.transform.position, getDirection(cam.transform.position, getPosition()), out hit, Mathf.Infinity, groundLayer))
        {
            v3 = hit.point;
            v3.x = Mathf.Round(Mathf.Round(v3.x) / gridSize) * gridSize;
            v3.z = Mathf.Round(Mathf.Round(v3.z) / gridSize) * gridSize;
        }
        return v3;
    }

    //retuns the first object the mouse is over
    public static GameObject getObject()
    {
        RaycastHit hit;
        GameObject g = null;
        if (Physics.Raycast(cam.transform.position, getDirection(cam.transform.position, getPosition()), out hit, Mathf.Infinity))
        {
            g = hit.transform.gameObject;
        }
        return g;
    }

    //checks if the mouse is over ui or not
    public static bool OverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<MouseUIClickthrough>() != null)
            {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }

        return raycastResultList.Count > 0;
    }

    //rechecks the collisions of each node if the grid
    public static void checkGridCollisions()
    {
        pathfinding.checkGridCollisions();
    }

    
}
