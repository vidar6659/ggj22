using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private LevelManager levelManager;

    public GameObject tilePrefab;
    public GameObject bobPrefab;
    public GameObject exitPrefab;
    public Material mirrorTileMat;

    private Player player;

    void Start()
    {
        levelManager = new LevelManager();
        levelManager.LoadLevel();
        levelManager.CreateLevel(tilePrefab, bobPrefab, exitPrefab, mirrorTileMat);
        player = new Player(levelManager);
    }
    
    void Update()
    {
        Act();
    }

    void Act()
    {
        player.Act();
    }

    public void EndLevel()
    {

    }
}
