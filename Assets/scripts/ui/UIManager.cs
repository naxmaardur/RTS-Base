using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]    
    GameObject buildingPlacer = null;                                                  //refernce to building placer object
    PlaceBuilding placerScript;                                                        //refernce to the place building script
    [SerializeField]
    GameObject templateButton = null;                                                  //a template button object
    [SerializeField]
    GameObject buyScreen = null;                                                       //canvas object where the player can buy buildings or units
    [SerializeField]
    RectTransform selectImg;                                                           //refernce to the drag selection image
    Vector3 startpos;                                                                  //starting positon of the selection
    Vector3 endpos;                                                                    //end position of the selection
    bool canSelect = true;                                                             //if the player can select
    // Start is called before the first frame update
    void Start()
    {
        placerScript = buildingPlacer.GetComponent<PlaceBuilding>();
        RenderBuildings();
    }

    // Update is called once per frame
    void Update()
    {
        if (canSelect)
        {
            SelectRect();
        }
    }

    //moves and rescales the selection image to the correct amounts
    void SelectRect()
    {
       
        if (Input.GetMouseButtonUp(0))
        {
            selectImg.gameObject.SetActive(false);
        }
        if (Input.GetMouseButton(0) && !Global.OverUI())
        {
            if (!selectImg.gameObject.activeInHierarchy && !Global.OverUI())
            {
                selectImg.gameObject.SetActive(true);
            }

            endpos = Input.mousePosition;

            Vector3 center = (startpos + endpos) / 2f;


            selectImg.position = center;

            float x = Mathf.Abs(startpos.x - endpos.x);
            float y = Mathf.Abs(startpos.y - endpos.y);

            selectImg.sizeDelta = new Vector2(x, y);
        }
    }

    //adds buttons for each building object to the buy canvas
    public void RenderBuildings()
    {
        ClearBuyScreen();
        foreach(BuildingObject b in Global.buildings)
        {
          GameObject btn = Instantiate(templateButton, buyScreen.transform);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = b.previewImg;
            btn.transform.GetChild(1).GetComponent<Text>().text = b.name;
            btn.transform.GetChild(2).GetComponent<Text>().text = b.cost.ToString();
            btn.transform.GetChild(3).GetComponent<Text>().text = b.buildTime.ToString();
            btn.transform.GetChild(4).GetComponent<Text>().text = b.health.ToString();
            btn.transform.GetChild(5).GetComponent<Text>().text = "";
            btn.GetComponent<Button>().onClick.AddListener(delegate { BuildingPlacer(b); });


        }
    }

    //adds buttons for each unit object to the buy canvas
    public void RenderUnitScreen(Building b)
    {
        ClearBuyScreen();
        foreach (UnitObject u in Global.units)
        {
            GameObject btn = Instantiate(templateButton, buyScreen.transform);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = u.previewImg;
            btn.transform.GetChild(1).GetComponent<Text>().text = u.name;
            btn.transform.GetChild(2).GetComponent<Text>().text = u.cost.ToString();
            btn.transform.GetChild(3).GetComponent<Text>().text = u.trainTime.ToString();
            btn.transform.GetChild(4).GetComponent<Text>().text = u.health.ToString();
            btn.transform.GetChild(5).GetComponent<Text>().text = u.attackDammage.ToString();
            ClickableObject CO = btn.AddComponent(typeof(ClickableObject)) as ClickableObject;
            CO.setBuilding(b);
            CO.setUnitObject(u);
            CO.setText(btn.transform.GetChild(6).GetComponent<Text>());
        }
    }

    //adds buttons for each unit object at or under a given level to the buy canvas
    public void RenderUnitScreen(Building b,int level)
    {
        ClearBuyScreen();
        foreach (UnitObject u in Global.units)
        {
            if (u.level <= level) {
                GameObject btn = Instantiate(templateButton, buyScreen.transform);
                btn.transform.GetChild(0).GetComponent<Image>().sprite = u.previewImg;
                btn.transform.GetChild(1).GetComponent<Text>().text = u.name;
                btn.transform.GetChild(2).GetComponent<Text>().text = u.cost.ToString();
                btn.transform.GetChild(3).GetComponent<Text>().text = u.trainTime.ToString();
                btn.transform.GetChild(4).GetComponent<Text>().text = u.health.ToString();
                btn.transform.GetChild(5).GetComponent<Text>().text = u.attackDammage.ToString();
                ClickableObject CO = btn.AddComponent(typeof(ClickableObject)) as ClickableObject;
                CO.setBuilding(b);
                CO.setUnitObject(u);
            }
        }
    }

    //removes all buttons form the buy canvas
    void ClearBuyScreen()
    {
        foreach(Transform child in buyScreen.transform)
        {
            if (child != buyScreen.transform)
            {
                Destroy(child.gameObject);
            }
        }
          
    }

    //sets the building object refernce of the building placer
    void BuildingPlacer(BuildingObject b)
    {
        StateMachine.SetGameState(StateMachine.GameStates.Construction);
        placerScript.setBuilding(b);
        buildingPlacer.SetActive(true);
    }

    //sets the starting position
    public void setStartPos(Vector3 pos)
    {
        startpos = pos;
    }

    //sets if the player can select
    public void setCanSelect(bool can)
    {
        canSelect = can;
    }



}
