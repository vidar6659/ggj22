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
        AsyncOperation[] aOps = new AsyncOperation[tiles.Length];
        int i = 0;
        bool notDone = true;
        foreach (var t in tiles)
        {
            NavMeshData nvd = t.GetComponent<NavMeshSurface>().navMeshData;
            aOps[i] = t.GetComponent<NavMeshSurface>().UpdateNavMesh(nvd);
            i++;
        }
        while (notDone)
        {
            notDone = false;
            foreach (AsyncOperation aop in aOps)
            {
                if (!aop.isDone)
                {
                    notDone = true;
                    break;
                }
            }
        }
    }

    public static void UpdateNavMesh(Vector2 aCoords, Vector2 bCoords)
    {
        string aName = aCoords.x + "," + aCoords.y;
        string bName = bCoords.x + "," + bCoords.y;
        foreach (var t in tiles)
        {
            if (t.transform.parent.name == aName || t.transform.parent.name == bName)
            {
                NavMeshData nvd = t.GetComponent<NavMeshSurface>().navMeshData;
                t.GetComponent<NavMeshSurface>().UpdateNavMesh(nvd);
            }
        }
    }

    public static void SetBobsDestination(Vector3 dest)
    {
        NavMeshAgent bobsNavMeshAgent = GameObject.Find("Bob").GetComponent<NavMeshAgent>();
        //NavMeshPath nvp = new NavMeshPath();
        //bool calcPathRes = bobsNavMeshAgent.CalculatePath(dest, nvp); //first calc
        //Debug.Log("first calc:" + calcPathRes);
        //Debug.Log("first calc:" + nvp.status);
        //GameObject.Find("Debugger").GetComponent<Debugger>().pathEndPos = bobsNavMeshAgent.pathEndPosition;
        //GameObject.Find("Debugger").GetComponent<Debugger>().pathCorners = nvp.corners;
        //nvp.ClearCorners();
        //calcPathRes = bobsNavMeshAgent.CalculatePath(dest, nvp);//first calc
        //Debug.Log("second calc:" + calcPathRes);
        //Debug.Log("second calc:" + nvp.status);
        //GameObject.Find("Debugger").GetComponent<Debugger>().pathEndPos = bobsNavMeshAgent.pathEndPosition;
        //GameObject.Find("Debugger").GetComponent<Debugger>().pathCorners = nvp.corners;
        //nvp.ClearCorners();
        //calcPathRes = bobsNavMeshAgent.CalculatePath(dest, nvp);//first calc
        //Debug.Log("third calc:" + calcPathRes);
        //Debug.Log("third calc:" + nvp.status);
        //GameObject.Find("Debugger").GetComponent<Debugger>().pathEndPos = bobsNavMeshAgent.pathEndPosition;
        //GameObject.Find("Debugger").GetComponent<Debugger>().pathCorners = nvp.corners;

        //if(nvp.corners[nvp.corners.Length - 1].x == dest.x && nvp.corners[nvp.corners.Length - 1].z == dest.z)
        //{
        //    bobsNavMeshAgent.SetDestination(dest);
        //}
        //if (nvp.status == NavMeshPathStatus.PathComplete)
        //{
        //    bobsNavMeshAgent.SetDestination(dest);
        //}
        bobsNavMeshAgent.SetDestination(dest);
    }


}
