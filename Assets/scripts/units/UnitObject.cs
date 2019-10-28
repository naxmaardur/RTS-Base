using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Unit", menuName = "RTS/Unit")]
public class UnitObject : ScriptableObject
{
    public new string name;
    public float health;
    public float attackDammage;
    public float attackSpeed = 3;
    public float range;
    public int cost;
    public float trainTime;
    public MeshObject MeshObject;
    public Sprite previewImg;
    public int level;
    public StateMachine.UnitTypes type;
}
