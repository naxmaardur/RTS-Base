using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class RayCastController : MonoBehaviour
{
    new BoxCollider collider;
    [HideInInspector]
    public int HRayCount = 4;
    [HideInInspector]
    public int VRayCount = 4;
    [HideInInspector]
    public int ZRayCount = 4;

    [HideInInspector]
    public float HRaySpacing;
    [HideInInspector]
    public float VRaySpacing;
    [HideInInspector]
    public float ZRaySpacing;
    public RaycastOrigins raycastOrigins;
    public float skinWidth = .015f;
    const float dstBetweenRays = .25f;


    Vector3 vertex1;
    Vector3 vertex2;
    Vector3 vertex3;
    Vector3 vertex4;

    public virtual void Awake()
    {
        collider = GetComponent<BoxCollider>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigings()
    {
        Vector3 size = collider.size;
        Vector3 center = new Vector3(collider.center.x, collider.center.y, collider.center.z);

        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeftFront = transform.TransformPoint(new Vector3((center.x - size.x / 2) + skinWidth * 2, (center.y - size.y / 2) + skinWidth * 2, (center.z + size.z / 2) - skinWidth * 2));
        raycastOrigins.bottomRightFront = transform.TransformPoint(new Vector3((center.x + size.x / 2) - skinWidth * 2, (center.y - size.y / 2) + skinWidth * 2, (center.z + size.z / 2) - skinWidth * 2));
        raycastOrigins.topLeftFront = transform.TransformPoint(new Vector3((center.x - size.x / 2) + skinWidth * 2, (center.y + size.y / 2) - skinWidth * 2, (center.z + size.z / 2) - skinWidth * 2));
        raycastOrigins.topRightFront = transform.TransformPoint(new Vector3((center.x + size.x / 2) - skinWidth * 2, (center.y + size.y / 2) - skinWidth * 2, (center.z + size.z / 2) - skinWidth * 2));

        raycastOrigins.bottomLeftBack = transform.TransformPoint(new Vector3((center.x - size.x / 2) + skinWidth * 2, (center.y - size.y / 2) + skinWidth * 2, (center.z - size.z / 2) + skinWidth * 2));
        raycastOrigins.bottomRightBack = transform.TransformPoint(new Vector3((center.x + size.x / 2) - skinWidth * 2, (center.y - size.y / 2) + skinWidth * 2, (center.z - size.z / 2) + skinWidth * 2));
        raycastOrigins.topLeftBack = transform.TransformPoint(new Vector3((center.x - size.x / 2) + skinWidth * 2, (center.y + size.y / 2) - skinWidth * 2, (center.z - size.z / 2) + skinWidth * 2));
        raycastOrigins.topRightBack = transform.TransformPoint(new Vector3((center.x + size.x / 2) - skinWidth * 2, (center.y + size.y / 2) - skinWidth * 2, (center.z - size.z / 2) + skinWidth * 2));
    }
    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;
        float boundsLength = bounds.size.z;

        HRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        VRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);
        ZRayCount = Mathf.RoundToInt(boundsLength / dstBetweenRays);

        HRayCount = Mathf.Clamp(HRayCount, 2, int.MaxValue);
        VRayCount = Mathf.Clamp(VRayCount, 2, int.MaxValue);
        ZRayCount = Mathf.Clamp(ZRayCount, 2, int.MaxValue);

        HRaySpacing = bounds.size.y / (HRayCount - 1);
        VRaySpacing = bounds.size.x / (VRayCount - 1);
        ZRaySpacing = bounds.size.z / (ZRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector3 topLeftFront, topRightFront;
        public Vector3 bottomLeftFront, bottomRightFront;
        public Vector3 topLeftBack, topRightBack;
        public Vector3 bottomLeftBack, bottomRightBack;
    }
}
