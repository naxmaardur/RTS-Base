using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Mesh", menuName = "Mesh/MeshObject")]
public class MeshObject : ScriptableObject
{
    public Mesh Mesh;
    public Material material;
    public Vector3 quaternion;
    public Vector3 size;
    public Vector3 offset;
}
