using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenuManager : MonoBehaviour
{
    private Vector3 tempPanelsPos;

    public GameObject inGamePanel;
    public GameObject pausePanel;
    public GameObject endPanel;

    void Start()
    {
        pausePanel.SetActive(false);
        endPanel.SetActive(false);
    }

    public void LoadEndPanel()
    {
        tempPanelsPos = inGamePanel.transform.position;
        inGamePanel.transform.position = endPanel.transform.position;
        endPanel.transform.position = tempPanelsPos;
        endPanel.SetActive(true);
    }

    public void LoadPausePanel()
    {
        Game.SetLevelPauseStatus(true);
        tempPanelsPos = inGamePanel.transform.position;
        inGamePanel.transform.position = pausePanel.transform.position;
        pausePanel.transform.position = tempPanelsPos;
        pausePanel.SetActive(true);
    }

    public void LoadInGamePanel()
    {
        tempPanelsPos = pausePanel.transform.position;
        pausePanel.transform.position = inGamePanel.transform.position;
        inGamePanel.transform.position = tempPanelsPos;
        pausePanel.SetActive(false);
        Game.SetLevelPauseStatus(false);
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void NextLevel()
    {
        GameObject.Find("GameManager").GetComponent<Game>().NextLevel();
        tempPanelsPos = endPanel.transform.position;
        endPanel.transform.position = inGamePanel.transform.position;
        inGamePanel.transform.position = tempPanelsPos;
        endPanel.SetActive(false);
        Game.SetLevelPauseStatus(false);
        Game.SetLevelIsCompleted(false);
    }

    public void ResetLevel()
    {
        GameObject.Find("GameManager").GetComponent<Game>().ResetLevel();
        LoadInGamePanel();
    }
}
