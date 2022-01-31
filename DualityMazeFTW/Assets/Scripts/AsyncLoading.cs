using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoading : MonoBehaviour
{
    private AsyncOperation loadingOperation;
    public Slider progressBar;
    private bool isLoading;

    void Start()
    {
        isLoading = false;
        if (progressBar == null)
            progressBar = GameObject.Find("LoadingSlider").GetComponent<Slider>();
    }

    public void LoadSceneAsync(string sceneToLoad)
    {
        progressBar = GameObject.Find("LoadingSlider").GetComponent<Slider>();
        loadingOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        isLoading = true;
    }

    void Update()
    {
        if (isLoading && loadingOperation != null && progressBar != null)
        {
            Debug.Log(loadingOperation.progress);
            progressBar.value = Mathf.Clamp01(loadingOperation.progress);
        }
        
    }
}
