using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using DG.Tweening;

public class LevelManager
{
    string path;
    string[] lines;
    int[][] logicLevel;
    int offset = 10;
    Vector3 tilePosition;
    Vector3 exitPosition;
    Vector2 exitCoord;
    Vector2 playerCoord;
    int[][] memoLevel;
    float bobMoveAnimDuration = 10.0f;
    List<(Vector2, Vector3)> pathToExit = new List<(Vector2, Vector3)>();

    const int WIDTH = 17;
    const int HEIGHT = 10;
    int levelWidth;
    int levelHeight;
    int xRoot = 0;
    int numberOfLevels;
    int indexLevels;

    GameObject bobRef;
    GameObject exitRef;
    List<GameObject> tilesRef = new List<GameObject>();

    GameObject tilePrefab;
    GameObject bobPrefab;
    GameObject exitPrefab;
    Material mirrorTileMat;

    public LevelManager(int number)
    {
        path = Application.dataPath + "/levels/" + "lvl_000.txt";
        tilePosition = new Vector3(0f, 0.1f, 0f);
        numberOfLevels = number;
        indexLevels = 0;
        Debug.Log(path);
    }

    private void ChangeFile()
    {
        string zeros = "";
        if (indexLevels < 10)
            zeros = "00";
        else if (indexLevels < 100)
            zeros = "";
        path = Application.dataPath + "/levels/lvl_" + zeros + indexLevels + ".txt";
    }

    public void LoadLevel(bool incrementIndexLevel)
    {
        if (File.Exists(path))
        {
            lines = File.ReadAllLines(path);
            levelHeight = lines.Length;
            int secondLength = levelWidth = lines[0].Split(',').Length;

            logicLevel = new int[lines.Length][];
            memoLevel = new int[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                logicLevel[i] = new int[lines[i].Split(',').Length];
                memoLevel[i] = new int[lines[i].Split(',').Length];
                logicLevel[i] = Array.ConvertAll(lines[i].Split(','), t => Int32.Parse(t));
            }
            SetInitialPosition();
        }
        if(incrementIndexLevel)
            indexLevels++;
    }

    private void SetInitialPosition()
    {
        xRoot = Mathf.CeilToInt((WIDTH - levelWidth) / 2);
        int zRoot = Mathf.CeilToInt((HEIGHT - levelHeight) / 2);
        tilePosition.x = xRoot * offset;
        tilePosition.z = -(zRoot * offset);
    }

    public void CreateLevel(GameObject _tilePrefab, GameObject _bobPrefab, GameObject _exitPrefab, Material _mirrorTileMat)
    {
        tilePrefab = _tilePrefab;
        bobPrefab = _bobPrefab;
        exitPrefab = _exitPrefab;
        mirrorTileMat = _mirrorTileMat;

        for (int i = 0; i < logicLevel.Length; i++)
        {
            for (int j = 0; j < logicLevel[i].Length; j++)
            {
                GameObject tile = GameObject.Instantiate(_tilePrefab);
                tile.transform.position = tilePosition;
                tile.transform.name = j.ToString() + "," + i.ToString();
                tile.GetComponent<Tile>().DefineTileStatus(logicLevel[i][j]);
                tile.GetComponent<Tile>().SetCoord(j, i);
                tilesRef.Add(tile);
                if (j >= levelWidth/2)
                {
                    tile.transform.GetChild(0).GetComponent<MeshRenderer>().materials[1].SetColor("_Color", new Color(0.245283f, 0.245283f, 0.245283f));
                }

                if (logicLevel[i][j] == 2) //player
                {
                    GameObject bob = GameObject.Instantiate(_bobPrefab);
                    bob.transform.name = "Bob";
                    Vector3 bobPosition = new Vector3(tilePosition.x, bob.transform.localScale.y / 2, tilePosition.z);
                    bob.transform.position = bobPosition;
                    playerCoord.x = j;
                    playerCoord.y = i;
                    bobRef = bob;
                    tile.GetComponent<Tile>().isStart = true;
                }
                else if (logicLevel[i][j] == 3) //exit
                {
                    GameObject exit = GameObject.Instantiate(_exitPrefab);
                    exit.transform.name = "Exit";
                    //exitPosition = new Vector3(tilePosition.x, exit.transform.localScale.y / 2, tilePosition.z);
                    exitPosition = new Vector3(tilePosition.x, tilePosition.y, tilePosition.z);
                    exit.transform.position = exitPosition;
                    exitCoord.x = j;
                    exitCoord.y = i;
                    exitRef = exit;
                    tile.GetComponent<Tile>().isExit = true;
                }
                tilePosition.x += offset;
            }
            tilePosition.x = xRoot * offset;
            tilePosition.z -= offset;
        }
        //NavMeshManager.CreateNavMesh();
        PrintLogicAndMemoLevels();
    }

    public Vector2 MirrorAction(Vector2 coord, bool block)
    {
        int mX = levelWidth - ((int)coord.x + 1);
        GameObject tile = GameObject.Find(mX + "," + coord.y);
        if (block)
        {
            logicLevel[(int)coord.y][mX] = 1;
            tile.GetComponent<Tile>().MakeBlock();
        }
        else
        {
            logicLevel[(int)coord.y][mX] = 0;
            tile.GetComponent<Tile>().MakeEmpty();
        }
        return new Vector2(mX, coord.y);
    }

    public void UpdateLogicLevel(Vector2 coord, int status)
    {
        logicLevel[(int)coord.y][(int)coord.x] = status;
        Vector2 mCoord = MirrorAction(coord, !Convert.ToBoolean(status));
        //NavMeshManager.UpdateNavMesh();
        //NavMeshManager.UpdateNavMesh(coord, mCoord);
        CloneMemoLogicLevel();
        PrintLogicAndMemoLevels();
        bool pathFound = IsPathToExitFound((int)playerCoord.x, (int)playerCoord.y);
        if (pathFound)
        {
            Game.isPathFound = true;
            pathToExit.Reverse();
            PrintPathToExit();
            AnimateBobPathToExit();
            //NavMeshManager.SetBobsDestination(exitPosition);
        }
        Debug.Log("Path has been found: " + pathFound);
    }

    private bool IsPathToExitFound(int x, int y)
    {
        if (!InsideBoundaries(x, y) || memoLevel[y][x] == 1)
            return false;
        if (memoLevel[y][x] == 3)
        {
            AddPathToExitEdge(x, y);
            return true;
        }

        memoLevel[y][x] = 1;
        if (IsPathToExitFound(x + 1, y))
        {
            AddPathToExitEdge(x, y);
            return true;
        }
        if (IsPathToExitFound(x, y - 1))
        {
            AddPathToExitEdge(x, y);
            return true;
        }
        if (IsPathToExitFound(x - 1, y))
        {
            AddPathToExitEdge(x, y);
            return true;
        }
        if (IsPathToExitFound(x, y + 1))
        {
            AddPathToExitEdge(x, y);
            return true;
        }

        return false;
    }

    private void CloneMemoLogicLevel()
    {
        for (int i = 0; i < logicLevel.Length; i++)
        {
            for (int j = 0; j < logicLevel[i].Length; j++)
            {
                memoLevel[i][j] = logicLevel[i][j];
            }
        }
    }

    private bool InsideBoundaries(int x, int y)
    {
        return (x >= 0 && x < memoLevel[0].Length && y >= 0 && y < memoLevel.Length);
    }

    public void ClearLevel()
    {
        GameObject.Destroy(bobRef);
        GameObject.Destroy(exitRef);
        foreach (GameObject t in tilesRef)
        {
            GameObject.Destroy(t);
        }
        Debug.Log("tilesRef.Length before clear: " + tilesRef.Count);
        tilesRef.Clear();
        Debug.Log("tilesRef.Length after clear: " + tilesRef.Count);
        pathToExit.Clear();
    }

    public bool ChangeToNextLevel()
    {
        if(indexLevels <= numberOfLevels - 1)
        {
            ClearLevel();
            ChangeFile();
            LoadLevel(true);
            CreateLevel(tilePrefab, bobPrefab, exitPrefab, mirrorTileMat);
            return true;
        }
        else
        {
            Debug.Log("Not enough levels");
        }
        return false;
    }

    private void AddPathToExitEdge(int x, int y)
    {
        Vector3 tilePos = GameObject.Find(x + "," + y).transform.position;
        pathToExit.Add((new Vector2(x, y), tilePos));
    }

    private void AnimateBobPathToExit()
    {
        float bobY = bobRef.transform.position.y;
        Vector3[] waypoints = new Vector3[pathToExit.Count];
        int i = 0;
        foreach ((Vector2, Vector3) coord in pathToExit)
            waypoints[i++] = new Vector3(coord.Item2.x, bobY, coord.Item2.z);
        bobRef.transform.DOPath(waypoints, bobMoveAnimDuration, gizmoColor: Color.yellow).SetLookAt(0.015f);
    }

    private void PrintLogicAndMemoLevels()
    {
        string levelsString = "";

        for (int i = 0; i < logicLevel.Length; i++)
            levelsString += String.Join("|", logicLevel[i]) + "\t" + String.Join("|", memoLevel[i]) + "\n";

        Debug.Log("logicLevel:\tmemoLevel:\n" + levelsString);
    }

    private void PrintPathToExit()
    {
        if (pathToExit.Count == 0)
            Debug.Log("Path to Exit not yet found");
        else
        {
            string pathString = "Path to Exit is ";
            string tilePosString = "Tile Positions are ";

            foreach ((Vector2, Vector3) coord in pathToExit)
                pathString += "-> (" + coord.Item1.x + "," + coord.Item1.y + ") ";
            foreach ((Vector2, Vector3) coord in pathToExit)
                tilePosString += "-> (" + coord.Item2.x + "," + coord.Item2.y + "," + coord.Item2.z + ") ";

            Debug.Log("Bob's position: " + bobRef.transform.position + "\n" + pathString + "\n" + tilePosString);
        }
    }

    public void ResetLevel()
    {
        ClearLevel();
        LoadLevel(false);
        CreateLevel(tilePrefab, bobPrefab, exitPrefab, mirrorTileMat);
    }

    public bool CheckIfTileIsInteractive(Vector2 coord)
    {
        if (logicLevel[(int)coord.y][(int)coord.x] == 2 ||
            logicLevel[(int)coord.y][(int)coord.x] == 3)
            return false;
        //Check mirror tile
        int mX = levelWidth - ((int)coord.x + 1);
        if (logicLevel[(int)coord.y][mX] == 2 ||
            logicLevel[(int)coord.y][mX] == 3)
            return false;
        return true;
    }
}
