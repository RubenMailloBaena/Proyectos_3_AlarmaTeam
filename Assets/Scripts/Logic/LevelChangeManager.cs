using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChangeManager : MonoBehaviour
{
    private static LevelChangeManager instance;
    
    [Header("Persistent")]
    [SerializeField] private SceneField gameplayScene;
    
    [Header("SCENES")] 
    [SerializeField] private SceneField mainMenu;
    [SerializeField] private List<SceneField> gameLevels;

    private int currentLevel = -1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public static LevelChangeManager GetInstance() => instance;

    public void StartGame(int levelIndex)
    {
        currentLevel = levelIndex - 1;
        SceneManager.UnloadSceneAsync(mainMenu);
        SceneManager.LoadSceneAsync(gameplayScene, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(gameLevels[currentLevel], LoadSceneMode.Additive);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(mainMenu, LoadSceneMode.Additive);
        UnloadAllGameLevels();
        SceneManager.UnloadSceneAsync(gameplayScene);
    }

    private void UnloadAllGameLevels()
    {
        for (int i = 0; i < gameLevels.Count; i++)
            if(IsSceneLoaded(gameLevels[i]))
                SceneManager.UnloadSceneAsync(gameLevels[i]);
    }

    private bool IsSceneLoaded(SceneField scene)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name.Equals(scene.SceneName))
                return true;
        }
        return false;
    }
}
