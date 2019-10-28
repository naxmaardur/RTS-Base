using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Base
{
    //=================base Building===================================================================
    new Renderer renderer;                                                             //reference to the mesh renderer
    MeshFilter filter;                                                                 //reference to the mesh filter
    BuildingObject buildingObject;                                                     //reference to which BuildingObject the building is using
    StateMachine.BuildingType type;                                                    //the type of building it is 
    Transform MeshObj;                                                                 //reference to the object rendering the mesh 
    [SerializeField]
    StateMachine.Players player;                                                       //the player the building belongs to
   //==================================================================================================

    //=================Unit Training===================================================================
    Transform spawn;                                                                   //unit spawning location
    Transform rallyPoint;                                                              //the position the units will walk to after training
    [SerializeField]
    GameObject unitPrefab = null;                                                      //base prefab of the units
    float constuctionTime;                                                             //the remaining time of the units traning
    List<List<UnitObject>> unitQue = new List<List<UnitObject>> { };                   //list of all units to be trained grouped by unitObject
    StateMachine.Training trainingState;                                               //the current state in the training prosses
    UnitObject unitObject;                                                             //the first unit To be trained
    //=================================================================================================


    //=================Storage========================================================================
    StateMachine.ResourceType ResourceType;                                            //the type of resource stored in the building
    float resourceAmount;                                                              //the amount of that resource stored in the building
    //=================================================================================================

    //=================base building===================================================================
    void Awake()
    {
        //getting the child objects
        MeshObj = transform.GetChild(0);
        spawn = transform.GetChild(1);
        rallyPoint = transform.GetChild(2);
        //getting the references required to set the mesh 
        renderer = MeshObj.GetComponent<Renderer>();
        filter = MeshObj.GetComponent<MeshFilter>();
    }
    //makes the path grid recheck its collisions and adds this building to a list in game master if the type is a storage(colector) building
    private void Start()
    {
        Global.checkGridCollisions();
        if (type == global::StateMachine.BuildingType.colector)
            Global.gm.addStorageToList(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(type == global::StateMachine.BuildingType.UBuilder)
        {
            if(unitQue.Count > 0)
            {
                Train();
            }
        }
    }

    //gets all building data from BuildingObject and then assigns it 
    public void GetBuilding(BuildingObject b)
    {
        buildingObject = b;
        try
        {
            filter.mesh = b.meshObject.Mesh;
            renderer.material = b.meshObject.material;
            MeshObj.localScale = b.meshObject.size;
            MeshObj.localPosition = b.meshObject.offset;
            MeshObj.rotation = Quaternion.Euler(b.meshObject.quaternion);
        }
        catch (Exception e)
        {

        }
        type = b.type;
        transform.localScale = new Vector3(b.size, b.size, b.size);

        currentHealth = b.health;
        maxHealth = b.health;
    }
    //returns the buildings type
    public global::StateMachine.BuildingType getType()
    {
        return type;
    }

    //start the correct fuction if the building is clicked on
    public void ClickedOn()
    {
        switch (type)
        {
            case global::StateMachine.BuildingType.house:

                break;
            case global::StateMachine.BuildingType.UBuilder:
                global::StateMachine.SetGameState(global::StateMachine.GameStates.InBuilding);
                Global.uiM.RenderUnitScreen(this);
                break;
            case global::StateMachine.BuildingType.colector:

                break;
            case global::StateMachine.BuildingType.turret:

                break;


        }
    }

    //start the correct fuction if the building sellected and then a right click happend
    public void RightClick()
    {
        switch (type)
        {
            case global::StateMachine.BuildingType.house:

                break;
            case global::StateMachine.BuildingType.UBuilder:
                rallyPoint.position = Global.getPointOnMap();
                break;
            case global::StateMachine.BuildingType.colector:

                break;
            case global::StateMachine.BuildingType.turret:

                break;


        }
    }

    //returns which player the building belongs to
    public global::StateMachine.Players getPlayer()
    {
        return player;
    }

    //retunrs the size of the building
    public float getSize()
    {
        return buildingObject.size;
    }

    //destroys this building and re checks the grid collisions
    protected override void DestroyThis()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        col.enabled = false;
        Global.checkGridCollisions();
        Global.gm.RemoveStorageToList(this);
        Destroy(gameObject);
    }
    //=================================================================================================

    //===============Unit Training Start=============================================================================================
    //adds the unitObject to the training que or unpauses the que if it was paused
    public void UnitToQue(UnitObject u)
    {
        if (trainingState == StateMachine.Training.idel || trainingState == StateMachine.Training.training)
        {
            bool found = false;
            if (unitQue.Count == 0)
            {
                unitQue.Add(new List<UnitObject> { u });
                return;
            }
            foreach (List<UnitObject> l in unitQue)
            {
                if (l[0] == u)
                {
                    l.Add(u);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                unitQue.Add(new List<UnitObject> { u });
            }
        } else
        {
            UnPause();
        }
    }

    //removes the unitObject from the que
    public void RemoveFromQue(UnitObject u)
    {
        if (unitQue.Count == 0)
        { 
            return;
        }
        foreach (List<UnitObject> l in unitQue)
        {
            if (l[0] == u)
            {
                l.Remove(u);
                if(l.Count == 0)
                {
                    unitQue.Remove(l);
                }
                return;
            }
        }
    }

    //returns the lenght of the unitObjects que
    public int getQueCount(UnitObject u)
    {
        foreach(List<UnitObject> l in unitQue)
        {
            if(l[0] == u)
            {
                return l.Count;
            }
        }
        return 0;
    }


    
    public void Train()
    {
        //checking if the first unit to be trained is still the same
        if(unitQue[0][0] != unitObject)
        {
            trainingState = StateMachine.Training.idel;
        }
        //if not training train first unit in que
        if (trainingState == StateMachine.Training.idel)
        {
            unitObject = unitQue[0][0];
            constuctionTime = unitQue[0][0].trainTime;
            trainingState = StateMachine.Training.training;
        }
        //training
        if(trainingState == StateMachine.Training.training)
        {
            constuctionTime -= Time.deltaTime;
            if(constuctionTime <= 0)
            {
                SpawnUnit(unitQue[0][0]);
            }
        }
    }

    //spawns a new unit at the spawn point and assaigns the unitObject to it then sends it to the rally point
    public void SpawnUnit(UnitObject u)
    {
        trainingState = StateMachine.Training.idel;
        GameObject unit = Instantiate(unitPrefab, spawn.transform.position, Quaternion.identity);
        unitController uController = unit.GetComponent<unitController>();
        uController.setUnitObject(u);
        uController.setTargetPosition(rallyPoint.position);

        unitQue[0].RemoveAt(0);
        if(unitQue[0].Count == 0)
        {
            unitQue.RemoveAt(0);
        }
    }
    //pauses the training of units
    public void Pause()
    {
        trainingState = StateMachine.Training.paused;
    }

    //unpauses the training of units
    public void UnPause()
    {
        if (constuctionTime > 0)
        {
            trainingState = StateMachine.Training.training;
        }
        else
        {
            trainingState = StateMachine.Training.idel;
        }
    }
    //=============================================================================================================================


    //===============Storage Building==============================================================================================
    //returns the resource type the building is storing
    public StateMachine.ResourceType getResourceType()
    {
        return ResourceType;
    }

    //trys to set the resource to a new type and returns if that happend
    public bool setResourceType(StateMachine.ResourceType type)
    {
        if(resourceAmount == 0)
        {
            ResourceType = type;
            return true;
        }
        return false;
    }

    //adds a amount to the stored amount
    public void addResource(float amount)
    {
        resourceAmount += amount;
    }
    //==============================================================================================================================
   
}
