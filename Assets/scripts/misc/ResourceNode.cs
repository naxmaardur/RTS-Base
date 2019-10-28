using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [SerializeField]
    StateMachine.ResourceType resource;


    public StateMachine.ResourceType getResourceType()
    {
        return resource;
    }
    public float getSize()
    {
        return transform.lossyScale.x;
    }
}
