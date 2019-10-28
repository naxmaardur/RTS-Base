using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    
    StateMachine.GameStates gameState;                                                 //the current state the game is in
    List<unitController> units = new List<unitController> { };                         //list of units to send commands
    StateMachine.Players CliendPlayer;                                                 //the player using this cliend
    Building currentBuilding;                                                          //the current sellected building                                                               

    [SerializeField]
    LayerMask selectionLayerMask;                                                      //layermask of what can be sellected
    Vector3 startpos;                                                                  //the starting position of a selection rect
    Vector3 endpos;                                                                    //the end position of a selection rect
    List<Building> storage = new List<Building>();                                     //list of all storage buildings
   
    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case StateMachine.GameStates.idel:
                if (!Global.OverUI() && Input.GetMouseButtonDown(0))
                {
                    ClickOnObject();
                }
                
                break;
            case StateMachine.GameStates.InBuilding:
                if(!Global.OverUI() && Input.GetMouseButtonDown(0))
                {
                    if (!ClickOnObject())
                    {
                        SetGameState(StateMachine.GameStates.idel);
                        currentBuilding = null;
                        Global.uiM.RenderBuildings();
                    }
                }
                break;
        }
        if(gameState != StateMachine.GameStates.paused)
        {
            if(!Global.OverUI() && Input.GetMouseButtonDown(0))
            {
                startpos = Input.mousePosition;
                Global.uiM.setStartPos(startpos);
                Global.uiM.setCanSelect(true);
            }
            if (!Global.OverUI() && Input.GetMouseButtonDown(1))
            {
                RightClickNormal();
            }
            if (Input.GetMouseButtonUp(0))
            {
                getSelection();
            }

        }
       
    }

    //check if the player has clicked on a object and then runs the coresponding functions for the object
    bool ClickOnObject()
    {
        GameObject g = Global.getObject();
        if (g != null)
        {
            Building b = g.GetComponent<Building>();
            if (b != null)
            {
                if (b.getPlayer() == CliendPlayer)
                {
                    currentBuilding = b;
                    b.ClickedOn();
                }
                return true;
            }
            unitController u = g.GetComponent<unitController>();
            if (u != null)
            {
                if (u.getPlayer() == CliendPlayer)
                {
                    tryRemoveUnits();
                    if(!units.Contains(u))
                        units.Add(u);
                }

                return true;
            }
            tryRemoveUnits();
            return false;
        }
        tryRemoveUnits();
        return false;
    }

    //checks if the player right clicked and then gives the coresponding command
    void RightClickNormal()
    {
        switch (gameState)
        {
            case StateMachine.GameStates.idel:
                if (units.Count > 0)
                {

                    GameObject g = Global.getObject();
                    if (g != null)
                    {
                        Terrain t = g.GetComponent<Terrain>();
                        if (t == null)
                        {

                            foreach (unitController unit in units)
                            {
                                unit.setTarget(g.transform);
                            }
                        }
                        else
                        {

                            Vector3 v = Global.getPointOnMap();
                            foreach (unitController u in units)
                            {
                                u.setTargetPosition(v);
                            }
                        }
                    }
                }
                break;

            case StateMachine.GameStates.InBuilding:
                if(currentBuilding != null)
                {
                    currentBuilding.RightClick();
                }
                break;
        }
       
    }

    //makes a sellection between start and end pos and then add all units(that belong to the player) in that sellection to the units to give commands
    void getSelection()
    {
        endpos = Global.cam.ScreenToViewportPoint(Input.mousePosition);
        startpos = Global.cam.ScreenToViewportPoint(startpos);
        if (endpos != startpos)
        {
            tryRemoveUnits();

            Rect r = new Rect(startpos.x, startpos.y, endpos.x - startpos.x, endpos.y - startpos.y);
            unitController[] controllers = GameObject.FindObjectsOfType<unitController>();

            foreach (unitController u in controllers)
            {
                if (r.Contains(Global.cam.WorldToViewportPoint(u.transform.position)))
                {
                    if (u.getPlayer() == CliendPlayer)
                    {
                        if (!units.Contains(u))
                        {
                            units.Add(u);
                        }
                    }
                }
            }


        }
    }

    //trys to clear the unit list
    void tryRemoveUnits()
    {
        if (!Input.GetKey(KeyCode.LeftControl))
        {
            units.Clear();
        }
    }

    //gives back the state the game is in
    public StateMachine.GameStates GetGameState()
    {
        return gameState;
    }

    //sets the state the game is in
    public void SetGameState(StateMachine.GameStates state)
    {
        gameState = state;
    }

    //adds given building to the list of buildings
    public void addStorageToList(Building b)
    {
        storage.Add(b);
    }

    //removes the given building from the list of buildings
    public void RemoveStorageToList(Building b)
    {
        storage.Remove(b);
    }

    //returns the list of buildings
    public List<Building> getStorageList()
    {
        return storage;
    }
}
