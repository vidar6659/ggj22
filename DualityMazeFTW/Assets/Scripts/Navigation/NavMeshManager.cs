using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshManager
{
    static GameObject[] tiles;

    public static void CreateNavMesh()
    {
        tiles = GameObject.FindGameObjectsWithTag("NavMeshTile");
        foreach (var t in tiles)
        {
            t.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }

    public static void UpdateNavMesh()
    {
        foreach (var t in tiles)
        {
            NavMeshData nvd = t.GetComponent<NavMeshSurface>().navMeshData;
            t.GetComponent<NavMeshSurface>().UpdateNavMesh(nvd);
        }
    }

    public static void SetBobsDestination(Vector3 dest)
    {
        NavMeshAgent bobsNavMeshAgent = GameObject.Find("Bob").GetComponent<NavMeshAgent>();
        NavMeshPath nvp = new NavMeshPath();
        bool calcPathRes = bobsNavMeshAgent.CalculatePath(dest, nvp);
        //Debug.Log(dest);
        Debug.Log(calcPathRes);
        Debug.Log(nvp.status);
        //Debug.Log(bobsNavMeshAgent.pathEndPosition);
        GameObject.Find("Debugger").GetComponent<Debugger>().pathEndPos = bobsNavMeshAgent.pathEndPosition;
        GameObject.Find("Debugger").GetComponent<Debugger>().pathCorners = nvp.corners;

        if (nvp.status == NavMeshPathStatus.PathComplete)
        {
            bobsNavMeshAgent.SetDestination(dest);
        }
    }


}
