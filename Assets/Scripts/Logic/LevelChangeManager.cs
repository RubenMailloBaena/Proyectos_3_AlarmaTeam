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
}
