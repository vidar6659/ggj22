using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

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

    public void LoadLevel()
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
                }
                tilePosition.x += offset;
            }
            tilePosition.x = xRoot * offset;
            tilePosition.z -= offset;
        }
        NavMeshManager.CreateNavMesh();

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
        bool pathFound = IsPathToDestFound((int)playerCoord.x, (int)playerCoord.y);
        if(pathFound)
            NavMeshManager.SetBobsDestination(exitPosition);
        Debug.Log("Path has been found: " + pathFound);
    }

    private bool IsPathToDestFound(int x, int y)
    {
        if (!InsideBoundaries(x, y) || memoLevel[y][x] == 1)
            return false;
        if (memoLevel[y][x] == 3)
            return true;

        memoLevel[y][x] = 1;
        if (IsPathToDestFound(x + 1, y))
            return true;
        if (IsPathToDestFound(x, y - 1))
            return true;
        if (IsPathToDestFound(x - 1, y))
            return true;
        if (IsPathToDestFound(x, y + 1))
            return true;

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
        tilesRef.Clear();
    }

    public bool ChangeToNextLevel()
    {
        if(indexLevels <= numberOfLevels - 1)
        {
            ClearLevel();
            ChangeFile();
            LoadLevel();
            CreateLevel(tilePrefab, bobPrefab, exitPrefab, mirrorTileMat);
            return true;
        }
        else
        {
            Debug.Log("Not enough levels");
        }
        return false;
    }
}
