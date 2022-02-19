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
    public static bool isPathfound = false;

    public int numberOfLevels;

    void Start()
    {
        levelManager = new LevelManager(numberOfLevels);
        levelManager.LoadLevel(true);
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

    public static void SetLevelIsCompleted(bool isLevelComp)
    {
        isLevelCompleted = isLevelComp;
    }

    public static void EndLevel()
    {
        isGamePaused = true;
        isLevelCompleted = true;
        GameObject.Find("Canvas").GetComponent<InGameMenuManager>().LoadEndPanel();
    }

    public void NextLevel()
    {
        levelManager.ChangeToNextLevel();
    }

    public void ResetLevel()
    {
        Debug.Log("reset level");
        levelManager.ResetLevel();
    }
}
