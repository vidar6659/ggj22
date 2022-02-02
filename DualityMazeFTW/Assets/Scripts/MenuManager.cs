using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    /// <summary>
    /// This method loads just one scene (just one) :/ --workprogress 4 hours
    /// </summary>
    ///
    public GameObject mainPanel;
    public GameObject loadingPanel;
    public GameObject howToPlayPanel;

    private Vector3 tempPanelsPos;

    public void LoadGameScene()
    {
        tempPanelsPos = mainPanel.transform.position;
        mainPanel.transform.position = loadingPanel.transform.position;
        loadingPanel.transform.position = tempPanelsPos;
        Game.StartLevel();
        transform.GetComponent<AsyncLoading>().LoadSceneAsync("GameScene");
    }

    public void LoadHowToPlayScene()
    {
        tempPanelsPos = mainPanel.transform.position;
        mainPanel.transform.position = howToPlayPanel.transform.position;
        howToPlayPanel.transform.position = tempPanelsPos;
    }

    public void LoadMainPanel()
    {
        tempPanelsPos = howToPlayPanel.transform.position;
        howToPlayPanel.transform.position = mainPanel.transform.position;
        mainPanel.transform.position = tempPanelsPos;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
