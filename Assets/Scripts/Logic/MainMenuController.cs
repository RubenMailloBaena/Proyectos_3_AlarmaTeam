using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private SceneField startbButtonScene;
    
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

    public void StartGame()
    {
        //gm.ChangeScene(startbButtonScene);
        SceneManager.LoadSceneAsync("Gameplay - Persistent", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(startbButtonScene, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void ExitGame()
    {
        gm.ExitGame();
    }
}
