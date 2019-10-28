using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Building", menuName = "RTS/Building")]
public class BuildingObject : ScriptableObject
{
    public new string name;
    public float health;
    public int cost;
    public float buildTime;
    public MeshObject meshObject;
    public Sprite previewImg;
    public float size;
    public StateMachine.BuildingType type;


}
