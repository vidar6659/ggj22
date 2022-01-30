using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public Vector3 pathEndPos; // test variable
    public Vector3[] pathCorners; // test variable

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(pathEndPos, 1);
        Gizmos.color = Color.red;
        if(pathCorners != null)
        {
            foreach (Vector3 corner in pathCorners)
            {
                Gizmos.DrawSphere(corner, 1);
            }
        }
    }
}
