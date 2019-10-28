using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBuilding : RayCastController
{
    [SerializeField]
    Material can = null;                                                               //refernce to the can place material
    [SerializeField]
    Material cannot = null;                                                            //refernce to the can't place material
    new Renderer renderer = null;                                                      //refernce to the mesh rendere
    public LayerMask Mask;                                                             //layer mask of which collions block buildings
    BuildingObject building;                                                           //refernce to which building object to place
    [SerializeField]
    GameObject buildingPrefab = null;                                                  //refernce to a base building object

    public float size = 1;                                                             //the size of the building
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    //moves the placer to the location of the mouse on the map
    //and places or cancels the placemend
    void Update()
    {
        if (!Global.OverUI() && StateMachine.GetGameState() == StateMachine.GameStates.Construction)
        {
            UpdateRaycastOrigings();
            Vector3 destination = Global.getPointOnMap();
            destination.y += transform.lossyScale.y / 2;
            transform.position = destination;
            setMaterial();
            if(Input.GetMouseButtonDown(0) && CanPlace())
            {
                placeBuilding();
            }
            if (Input.GetMouseButtonDown(1))
            {
                gameObject.SetActive(false);
            }
        }
        
    }

    //every time the placer is enabled
    public void OnEnable()
    {
        transform.localScale = new Vector3(size, size, size);
        CalculateRaySpacing();
    }

    //returns if the building can beplaced at he current position
    bool CanPlace()
    {
        Collider[] cols = Physics.OverlapBox(transform.position, transform.lossyScale / 2, Quaternion.identity, Mask);
        if(cols.Length != 0)
                return false;
        if (CheckBelow() > 0.9f)
            return false;
        return true;
    }

    //changes the material based on if the building can be placed or not
    void setMaterial()
    {
        renderer.material = CanPlace() ? can : cannot;
    }

    //returns the fardest most ray hit below the placer
    float CheckBelow()
    {
        float directionY = -1;
        float rayLength = 200 + skinWidth;
        float bigestDist = 0;

        for (int i2 = 0; i2 < ZRayCount; i2++)
        {
            for (int i = 0; i < VRayCount; i++)
            {
                Vector3 rayOrigin = raycastOrigins.bottomLeftBack;
                rayOrigin += transform.forward * (ZRaySpacing * i2);
                rayOrigin += transform.right * (VRaySpacing * i);
                RaycastHit hit;
                Physics.Raycast(rayOrigin, transform.up * directionY, out hit, rayLength, Global.groundLayer);
                Debug.DrawRay(rayOrigin, transform.up * directionY, Color.red);
                if (hit.collider)
                {
                    float newDist = Vector3.Distance(hit.point, rayOrigin);
                    if(newDist > bigestDist)
                    {
                        bigestDist = newDist;
                    }
                }
            }
        }
       
        return bigestDist;
    }

    //sets the building object refrence
    public void setBuilding(BuildingObject b)
    {
        building = b;
        size = b.size;
    }

    //instantiates a new building at the placer positon and asigns the building object to it
    public void placeBuilding()
    {
       GameObject b = Instantiate(buildingPrefab, transform.position, Quaternion.identity);
        Building bu = b.GetComponent<Building>();
       
        bu.GetBuilding(building);
        StateMachine.SetGameState(StateMachine.GameStates.idel);
        gameObject.SetActive(false);

    }
}
