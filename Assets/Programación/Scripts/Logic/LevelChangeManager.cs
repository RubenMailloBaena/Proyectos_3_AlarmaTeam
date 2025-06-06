using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using UnityEngine.UI;

public class LevelChangeManager : MonoBehaviour
{
    private static LevelChangeManager instance;
    
    [Header("Persistent")]
    [SerializeField] private SceneField gameplayScene;
    
    [Header("SCENES")] 
    [SerializeField] private SceneField mainMenu;
    [SerializeField] private SceneField levelSelector;
    [SerializeField] private List<SceneField> gameLevels;

    [Header("Loading Screen")] 
    [SerializeField] private Animator loadingScreen;
    

    private int currentLevel = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        RuntimeManager.LoadBank("Master", true);
        RuntimeManager.LoadBank("Master.strings", true);
    }
    public static LevelChangeManager GetInstance() => instance;

    public void StartGame(int levelIndex)
    {
        Time.timeScale = 1.0f;
        currentLevel = levelIndex - 1;
        
        StartCoroutine(LoadStartLevel());
    }

    public bool isLastLevel()
    {
        print("cLvl: " + currentLevel);
        print("count: " + gameLevels.Count);
        return currentLevel == gameLevels.Count - 1;
    }

    private IEnumerator LoadStartLevel()
    {
        loadingScreen.SetTrigger("StartScreen");

        yield return new WaitForSeconds(0.5f);

        if (IsSceneLoaded(mainMenu))
            yield return SceneManager.UnloadSceneAsync(mainMenu);
        else
            yield return SceneManager.UnloadSceneAsync(levelSelector);

        yield return SceneManager.LoadSceneAsync(gameplayScene, LoadSceneMode.Additive);
        yield return SceneManager.LoadSceneAsync(gameLevels[currentLevel], LoadSceneMode.Additive);

        yield return null;

        loadingScreen.SetTrigger("StopScreen");
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
        GameManager.GetInstance().GetPlayerHUD().DisableRestartLevel(true);
    }

    public void UnloadPreviousLevel()
    {
        if (currentLevel - 1 < 0) return;

        if(IsSceneLoaded(gameLevels[currentLevel - 1]))
            SceneManager.UnloadSceneAsync(gameLevels[currentLevel - 1]);
        GameManager.GetInstance().GetPlayerHUD().DisableRestartLevel(false);
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
