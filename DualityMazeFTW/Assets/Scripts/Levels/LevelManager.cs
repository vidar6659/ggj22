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

    const int WIDTH = 17;
    const int HEIGHT = 10;
    int levelWidth;
    int levelHeight;
    int xRoot = 0;

    public LevelManager()
    {
        path = Application.dataPath + "/levels/" + "lvl_000.txt";
        tilePosition = new Vector3(0f, 0.1f, 0f);
        Debug.Log(path);
    }

    private void ChangeFile(string file)
    {
        path = Application.dataPath + "/levels/" + file;
    }

    public void LoadLevel()
    {
        if (File.Exists(path))
        {
            lines = File.ReadAllLines(path);
            levelHeight = lines.Length;
            int secondLength = levelWidth = lines[0].Split(',').Length;

            logicLevel = new int[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                logicLevel[i] = new int[lines[i].Split(',').Length];
                logicLevel[i] = Array.ConvertAll(lines[i].Split(','), t => Int32.Parse(t));
                
            }
            SetInitialPosition();
        }
    }

    private void SetInitialPosition()
    {
        xRoot = Mathf.CeilToInt((WIDTH - levelWidth) / 2);
        int zRoot = Mathf.CeilToInt((HEIGHT - levelHeight) / 2);
        tilePosition.x = xRoot * offset;
        tilePosition.z = -(zRoot * offset);
    }

    public void CreateLevel(GameObject tilePrefab, GameObject bobPrefab, GameObject exitPrefab)
    {
        for (int i = 0; i < logicLevel.Length; i++)
        {
            for (int j = 0; j < logicLevel[i].Length; j++)
            {
                GameObject tile = GameObject.Instantiate(tilePrefab);
                tile.transform.position = tilePosition;
                tile.transform.name = j.ToString() + "," + i.ToString();
                tile.GetComponent<Tile>().DefineTileStatus(logicLevel[i][j]);
                tile.GetComponent<Tile>().SetCoord(j, i);
                if (logicLevel[i][j] == 2) //player
                {
                    GameObject bob = GameObject.Instantiate(bobPrefab);
                    bob.transform.name = "Bob";
                    Vector3 bobPosition = new Vector3(tilePosition.x, bob.transform.localScale.y/2, tilePosition.z);
                    bob.transform.position = bobPosition;
                }
                else if (logicLevel[i][j] == 3) //exit
                {
                    GameObject exit = GameObject.Instantiate(exitPrefab);
                    //exitPosition = new Vector3(tilePosition.x, exit.transform.localScale.y / 2, tilePosition.z);
                    exitPosition = new Vector3(tilePosition.x, tilePosition.y, tilePosition.z);
                    exit.transform.position = exitPosition;
                }
                tilePosition.x += offset;
            }
            tilePosition.x = xRoot * offset;
            tilePosition.z -= offset;
        }
        NavMeshManager.CreateNavMesh();

    }

    public void MirrorAction(Vector2 coord, bool block)
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
    }

    public void UpdateLogicLevel(Vector2 coord, int status)
    {
        logicLevel[(int)coord.y][(int)coord.x] = status;
        MirrorAction(coord, !Convert.ToBoolean(status));
        NavMeshManager.UpdateNavMesh();
        NavMeshManager.SetBobsDestination(exitPosition);
    }
}
