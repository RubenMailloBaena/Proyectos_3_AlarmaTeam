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
    [SerializeField] private SceneField levelSelector;
    [SerializeField] private List<SceneField> gameLevels;

    private int currentLevel = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public static LevelChangeManager GetInstance() => instance;

    public void StartGame(int levelIndex)
    {
        Time.timeScale = 1.0f;
        currentLevel = levelIndex - 1;
        if (IsSceneLoaded(mainMenu))
            SceneManager.UnloadSceneAsync(mainMenu);
        else
            SceneManager.UnloadSceneAsync(levelSelector);
        SceneManager.LoadSceneAsync(gameplayScene, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(gameLevels[currentLevel], LoadSceneMode.Additive);
    }

    public void LoadNextLevel()
    {
        if (currentLevel + 1 >= gameLevels.Count)
        {
            Debug.LogWarning("THERE ARE NOT MORE LEVELS");
            return;
        }
        
        currentLevel++;
        SceneManager.LoadSceneAsync(gameLevels[currentLevel], LoadSceneMode.Additive);
    }

    public void UnloadPreviousLevel()
    {
        if (currentLevel - 1 < 0) return;

        if(IsSceneLoaded(gameLevels[currentLevel - 1]))
            SceneManager.UnloadSceneAsync(gameLevels[currentLevel - 1]);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1.0f;
        GameManager.GetInstance().SetCursorVisible(true);
        SceneManager.LoadSceneAsync(mainMenu, LoadSceneMode.Additive);
        UnloadAllGameLevels();
        if (IsSceneLoaded(gameplayScene))
            SceneManager.UnloadSceneAsync(gameplayScene);
        if (IsSceneLoaded(levelSelector))
            SceneManager.UnloadSceneAsync(levelSelector);
    }

    public void GoToLevelSelector()
    {
        SceneManager.LoadSceneAsync(levelSelector, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(mainMenu);
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
