using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Parent,
    Child
}

public class WaypointGizmo : MonoBehaviour
{
    public Type type;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (type == Type.Child)
            Gizmos.DrawSphere(transform.position, 0.1f);
        else if (type == Type.Parent)
            Gizmos.DrawCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));
    }
}
