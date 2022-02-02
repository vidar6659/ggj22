using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private Player player;
    private LevelManager levelManager;

    public GameObject tilePrefab;
    public GameObject bobPrefab;
    public GameObject exitPrefab;
    public Material mirrorTileMat;
    public static bool isGamePaused = false;
    public static bool isLevelCompleted = false;

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

    public static void StartLevel()
    {
        isLevelCompleted = false;
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    public static void SetLevelPauseStatus(bool pauseGame)
    {
        isGamePaused = pauseGame;
        Time.timeScale = (pauseGame) ? 0f : 1f;
    }

    public static void EndLevel()
    {
        isGamePaused = true;
        isLevelCompleted = true;
        GameObject.Find("Canvas").GetComponent<InGameMenuManager>().LoadEndPanel();
    }
}
