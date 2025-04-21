using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private AsyncOperation loadLogicScene;
    private GameManager gm;
    
    private void Awake()
    {
        loadLogicScene = SceneManager.LoadSceneAsync("Logic - Persistent", LoadSceneMode.Additive);
        StartCoroutine(LoadingDone());
    }

    private IEnumerator LoadingDone()
    {
        while (!loadLogicScene.isDone)
        {
            print(loadLogicScene.progress);
            yield return null;
        }

        gm = GameManager.GetInstance();
    }

    public void StartGame(string scene)
    {
        gm.ChangeScene(scene);
    }

    public void ExitGame()
    {
        gm.ExitGame();
    }
}
