using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitController : PathfindingAgent
{
    //=================Base unit=======================================================================
    Transform MeshObj;                                                                 //reference to the object rendering the mesh 
    new Renderer renderer;                                                             //reference to the mesh renderer
    MeshFilter filter;                                                                 //reference to the mesh filter
    UnitObject unitObject;                                                             //reference to which unitObject the unit is using;
    Transform target;                                                                  //the current Transform the unit is moving to if not null
    StateMachine.Players player;                                                       //the player the unit belongs to
    StateMachine.UnitStates state;                                                     //current state of the unit
    StateMachine.UnitTypes type;                                                       //the type of unit it is
    //=================================================================================================

    //=================combat unit=====================================================================
    [SerializeField]
    LayerMask hitMask = 0;                                                             //the layermask used to check if unit can hit
    [SerializeField]
    LayerMask checkMask;                                                               //the layermask used to check if there is a enemy near the unit
    Vector3 gaurdingPosition;                                                          //the position the unit will gaurd when in the gaurd state
    Transform tempTarget;                                                              //a temporary target reference
    [SerializeField]
    List<Transform> ignore = new List<Transform>();                                    //list of units and buildings to ignore
    float maxFollow = 20;                                                              //the maximum distance the unit can go away from the gaurding position
    bool canAttack = true;                                                             //if the unit can attack
    float attackDammage;                                                               //amount of dammage done during a attack
    float attackSpeed;                                                                 //the max speed a unit can attack at
    float range;                                                                       //the max distance that the unit can still attack
    //=================================================================================================

    //=================worker unit=====================================================================
    float collectionTime = 0.1f;                                                        //time it takes to collect a resource
    int collectedAmount;                                                                //the amount of the current resource the unit is carrying
    StateMachine.ResourceType resource;                                                 //the current resource type the unit is collecting
    ResourceNode LastReSourceNode;                                                      //the last resource node fisited by the unit
    Building LastStorage;                                                               //the last Storage building fisited by the unit
    ResourceNode currentResourceNode;                                                   //the resource node the unit is currently going to
    Building currentStorage;                                                            //the storage building the unit is currently going to
    Transform OverRideTransform;                                                        //a transform that will override the units targets after clearing its resources 
    float currentTime;                                                                  //the current progress of collecting at the current resource node
    //=================================================================================================

    //=================Base unit=======================================================================
    void Awake()
    {
        //getting the child objects
        MeshObj = transform.GetChild(0);
        renderer = MeshObj.GetComponent<Renderer>();
        filter = MeshObj.GetComponent<MeshFilter>();
    }

    protected override void Start()
    {
        base.Start();
        gaurdingPosition = targetPosition;
    }
    // Update is called once per frame
    //the correct behavior is called based on the unit type and the pathfinding is followed and calculated on the parent
    new void Update()
    {
        switch (type)
        {
            case StateMachine.UnitTypes.combat:
                combat();
                break;
            case StateMachine.UnitTypes.worker:
                worker();
                break;
        }

        if (selfNode != null && targetNode != null && state != StateMachine.UnitStates.idel)
        {
            followPath = (Global.pathfinding.GetDistance(selfNode, targetNode) > range);
        }
        base.Update();
    }

    //setting the position on the map to move to
    public void setTargetPosition(Vector3 v)
    {
        targetPosition = v;
        target = null;
        state = StateMachine.UnitStates.MovingToPoint;
    }

    //calling the correct target set function based on unit type
    public void setTarget(Transform t)
    {
        switch (type) {

            case StateMachine.UnitTypes.combat:
                SetCombatTarget(t);
                break;

            case StateMachine.UnitTypes.worker:
                setWorkerTarget(t);
                break;
        }

           
       
    }

    //sets the units variables to the ones of the unit object 
    public void setUnitObject(UnitObject u)
    {
        unitObject = u;
        currentHealth = u.health;
        maxHealth = u.health;
        attackDammage = u.attackDammage;
        range = u.range;
        attackSpeed = u.attackSpeed;
        type = u.type;
        try
        {
            filter.mesh = u.MeshObject.Mesh;
            renderer.material = u.MeshObject.material;
            MeshObj.localScale = u.MeshObject.size;
            MeshObj.localPosition = u.MeshObject.offset;
            MeshObj.rotation = Quaternion.Euler(u.MeshObject.quaternion);
        }
        catch (Exception e)
        {

        }

    }

    //changing the state of the unit at the end of a path
    protected override void EndOffPath()
    {
        base.EndOffPath();
        if (state == StateMachine.UnitStates.MovingToPoint)
        {
            switch (type)
            {
                case StateMachine.UnitTypes.combat:
                    gaurdingPosition = transform.position;
                    state = StateMachine.UnitStates.guarding;
                    break;
                case StateMachine.UnitTypes.worker:
                    state = StateMachine.UnitStates.idel;
                    break;
            }
        }
    }

    //returns which player the unit belongs to
    public StateMachine.Players getPlayer()
    {
        return player;
    }
    //=================================================================================================

    //=================combat unit=====================================================================

    void combat()
    {
        if (target != null)
        {
            if (state == StateMachine.UnitStates.attacking)
            {
                tempTarget = null;

                targetPosition = target.position;
                TryToAttack(target);
            }

        }

        if (state == StateMachine.UnitStates.guarding || state == StateMachine.UnitStates.MovingToPoint)
        {
            LookForEnemys();
            AttackTempTarget();
        }
    }
    //trys to attack a enemy in range if there is one and moves towards it if the unit is in the guarding state
    void AttackTempTarget()
    {
        if (tempTarget != null)
        {
            TryToAttack(tempTarget);
            if (state == StateMachine.UnitStates.guarding)
            {
                targetPosition = tempTarget.position;
                if (Vector3.Distance(transform.position, gaurdingPosition) > maxFollow)
                {
                    tempTarget = null;
                    targetPosition = gaurdingPosition;
                }
            }
        }
        else
        {
            if (state == StateMachine.UnitStates.guarding)
            {
                targetPosition = gaurdingPosition;
            }
        }

    }


    //checks in the units attack range if there is a enemy in it if so it stores it in the tempTarget and alies are added to the ignore list so they are no longer checked
    void LookForEnemys()
    {
        foreach(Collider c in Physics.OverlapSphere(transform.position, range, checkMask))
        {
            if (!ignore.Contains(c.transform))
            {
                unitController u = c.GetComponent<unitController>();
                Building b = c.GetComponent<Building>();

                if (u != null)
                {
                    if (u.player != player)
                    {
                        tempTarget = c.transform;
                        return;
                    }
                    else
                        ignore.Add(c.transform);
                }
                if(b != null)
                {
                    if (b.getPlayer() != player)
                    {
                        tempTarget = c.transform;
                        return;
                    }
                    else
                        ignore.Add(c.transform);
                }
            }
           
        }
        tempTarget = null;
    }
    
    //trys to attack the targeted enemy if it is in range and can directly be seen only counting objects on the layermask
    void TryToAttack(Transform t)
    {
        if (Vector3.Distance(transform.position, t.position) <= range && canAttack){
            Vector3 direction = (t.position - transform.position);
            RaycastHit hit;
            if(Physics.Raycast(transform.position, direction.normalized,out hit, range, hitMask))
            {
               unitController unitTarget = hit.collider.gameObject.GetComponent<unitController>();
              
                if (unitTarget != null)
                {
                    if(unitTarget.getPlayer() != player)
                    {
                        StartCoroutine(attack());
                        unitTarget.takeDamge(attackDammage);
                    }
                       
                   
                    return;
                }
                Building buildingTarget = hit.collider.gameObject.GetComponent<Building>();
                if (buildingTarget != null)
                {
                    if (buildingTarget.getPlayer() != player)
                    {
                        StartCoroutine(attack());
                        buildingTarget.takeDamge(attackDammage);
                    }
                      
                }
            }
        }
    }

    //makes sure the unit can only attack at its attackSpeed
    IEnumerator attack()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackSpeed);
        canAttack = true;
    }

    //checks the target transform if it is a attackable object if so attack it else move towards the transforms current world position
    void SetCombatTarget(Transform t)
    {
        unitController unitTarget = t.GetComponent<unitController>();
        Building buildingTarget = t.GetComponent<Building>();
        if(buildingTarget != null || unitTarget != null)
        {

        }
        if ((buildingTarget != null && buildingTarget.getPlayer() != player)||(unitTarget != null && unitTarget.getPlayer() != player ))
        {
            target = t;
            state = StateMachine.UnitStates.attacking;
        }
        else
            setTargetPosition(t.position);
    }
    //=================================================================================================

    //=================Worker unit=====================================================================
    void worker()
    {
        if(currentStorage != null || currentResourceNode != null)
        {
            CollectionProcess();
        } else if (target != null)
        {
            getCorrectTarget();
           
        }
        if(target !=null)
            targetPosition = target.position;

    }

    //makes sure that the unit moves to the correct object in the collection loop
    void getCorrectTarget()
    {
        Building b = target.GetComponent<Building>();
        if (b != null)
        {
            LastStorage = b;
            resource = b.getResourceType();
            if (collectedAmount != 0)
            {
                currentStorage = b;
                state = StateMachine.UnitStates.MovingToCollectionPoints;
                return;
            }
            else
            {
                currentResourceNode = getClossestResourceNode(resource, b.transform.position);
                target = currentResourceNode.transform;
                state = StateMachine.UnitStates.MovingToCollectionPoints;
                return;
            }
        }
        ResourceNode r = target.GetComponent<ResourceNode>();
        if (r != null)
        {
            LastReSourceNode = r;
            resource = r.getResourceType();
            currentResourceNode = r;
        }
        state = StateMachine.UnitStates.MovingToCollectionPoints;
    }

    void CollectionProcess()
    {
        if (currentStorage != null)
            HandelBuilding();
        if (currentResourceNode != null)
            HandelResourceNode();
    }

    //if the unit is in range to collect resources collect resources
    void HandelResourceNode()
    {
        if (Vector3.Distance(currentResourceNode.transform.position, transform.position) <= currentResourceNode.getSize() + 0.4f)
        {
            if (state == StateMachine.UnitStates.MovingToCollectionPoints)
            {
                currentTime = collectionTime;
                state = StateMachine.UnitStates.collecting;
            }
            if(state == StateMachine.UnitStates.collecting)
            {
               
                currentTime -= Time.deltaTime;
                if (currentTime <= 0)
                    doneCollecting();
            }
            
        }
    }
    //if the unit is in range to deliver the resource delivers it and gos to the next target in the loop if not overriden
    void HandelBuilding()
    {
        if(Vector3.Distance(currentStorage.transform.position,transform.position)<= currentStorage.getSize() + 0.4f)
        {
            if (IsCorrectResource(currentStorage))
            {
                currentStorage.addResource(collectedAmount);
                collectedAmount = 0;
                LastStorage = currentStorage;
                currentStorage = null;
                if (OverRideTransform == null)
                {
                    if (LastReSourceNode != null)
                    {
                        currentResourceNode = LastReSourceNode;
                        target = LastReSourceNode.transform;
                        return;
                    }
                    else
                    {
                        currentResourceNode = getClossestResourceNode(LastStorage.getResourceType(), LastStorage.transform.position);
                        target = currentResourceNode.transform;
                        return;
                    }
                }
                clearLocationReferences();
                target = OverRideTransform;
                OverRideTransform = null;
            }
            else
            {
                currentStorage = getClossestStorage(resource, transform.position);
                target = currentStorage.transform;
            }
        }
    }
    //adds resource to unit and sets target to next one in the colection loop
    void doneCollecting()
    {
       
        state = StateMachine.UnitStates.MovingToCollectionPoints;
        collectedAmount = 10;
        LastReSourceNode = currentResourceNode;
        currentResourceNode = null;
        if (LastStorage != null)
        {
            currentStorage = LastStorage;
            target = LastStorage.transform;
            return;
        }
        else
        {
            currentStorage = getClossestStorage(LastReSourceNode.getResourceType(), LastReSourceNode.transform.position);
            target = currentStorage.transform;
            return;
        }
    }

    //removes all references to the old resource loop
    void clearLocationReferences()
    {
        LastReSourceNode = null;
        LastStorage = null;
        currentStorage = null;
        currentResourceNode = null;
    }

    //loops trough all storage buildings and gives back the closest storage building with the same resource as the unit to the world position given
    Building getClossestStorage(StateMachine.ResourceType type,Vector3 from)
    {
        float distance = 300000000000000000;
        Building building = null;
       
        
        foreach (Building b in Global.gm.getStorageList())
        {
                if ((b.getResourceType() == type || IsCorrectResource(b)))
                {
                    if (Vector3.Distance(b.transform.position, from) < distance)
                    {
                        building = b;
                    }
                }
        }


        return building;
    }

    //loops trough all resource nodes and gives back the closest with the same resource as the unit to the world position given
    ResourceNode getClossestResourceNode(StateMachine.ResourceType type, Vector3 from)
    {
        float distance = 300000000000000000;
        ResourceNode node = null;
        ResourceNode[] nodes = GameObject.FindObjectsOfType<ResourceNode>();
        foreach(ResourceNode n in nodes)
        {
            if(n.getResourceType() == type)
            {
                if(Vector3.Distance(n.transform.position,from) < distance)
                {
                    node = n;
                }
            }
        }


        return node;
    }

    //starts the behavior the change the units resource type 
    void ChangeResourceType(StateMachine.ResourceType resource, Transform target)
    {
        if(collectedAmount > 0)
        {
            OverRideTransform = target;
            if (LastStorage != null)
            {
                Debug.Log(LastStorage);
                currentStorage = LastStorage;
                this.target = currentStorage.transform;
                Debug.Log(target);
            }
            else
            {
                currentStorage = getClossestStorage(this.resource, transform.position);
                this.target = currentStorage.transform;
            }
                       currentResourceNode = null;
        }
        else
        {
            this.resource = resource;
            this.target = target;
            getCorrectTarget();
        }
    }

    //checks the target transform if it is a resource loop object if so try to set it or the unit to the resource type 
    //else move towards the transforms current world position
    void setWorkerTarget(Transform t)
    {
        Building buildingTarget = t.GetComponent<Building>();
        ResourceNode resourceNode = t.GetComponent<ResourceNode>();
        if (buildingTarget != null)
        {
            if (buildingTarget.getPlayer() == player && buildingTarget.getType() == StateMachine.BuildingType.colector)
            {
                if (IsCorrectResource(buildingTarget))
                {
                    target = t;
                    getCorrectTarget();

                }
                else
                    ChangeResourceType(buildingTarget.getResourceType(), t);
            }
        }
        else if (resourceNode != null)
        {
            if (IsCorrectResource(resourceNode))
            {
                target = t;
                getCorrectTarget();
            }
            else
                ChangeResourceType(resourceNode.getResourceType(), t);
        }
        else
        {
            setTargetPosition(t.position);
        }



    }

    //checks if the buildings has the same resource type as the unit if not can it be changed to the same type
    bool IsCorrectResource(Building buildingTarget)
    {
        if (buildingTarget.getResourceType() == resource)  
            return true;
        return buildingTarget.setResourceType(resource);
    }
    //checks if the resource node has the same resource type as the unit
    bool IsCorrectResource(ResourceNode resourceNode)
    {
        if (resourceNode.getResourceType() == resource)
            return true;
        return false;
    }
    //==================================================================================================
}
