using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour, IPointerClickHandler
{
    Building b;
    UnitObject u;
    Text text;

    void LateUpdate()
    {
       changeText(b.getQueCount(u));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            b.UnitToQue(u);
        else if (eventData.button == PointerEventData.InputButton.Middle)
            b.Pause();
        else if (eventData.button == PointerEventData.InputButton.Right)
            b.RemoveFromQue(u);
    }

    void changeText(int i)
    {
        if(i > 0)
            text.text = "" + i;
        if(i == 0)
            text.text = "";
    }

    public void setBuilding(Building b)
    {
        this.b = b;
    }
    public void setUnitObject(UnitObject u)
    {
        this.u = u;
    }
    public void setText(Text text)
    {
        this.text = text;
    }
}  

