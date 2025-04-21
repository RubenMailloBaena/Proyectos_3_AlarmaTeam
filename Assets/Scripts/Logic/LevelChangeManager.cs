using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChangeManager : MonoBehaviour
{
    [Header("SCENES")] 
    [SerializeField] private SceneField mainMenu;
    [SerializeField] private SceneField Level1;
    [SerializeField] private SceneField Level2;
    
    [Header("Persistent")]
    [SerializeField] private SceneField gameplayScene;
    
    public void StartGame(SceneField scene)
    {
        SceneManager.UnloadSceneAsync(mainMenu);
        SceneManager.LoadSceneAsync(gameplayScene, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
    }
}
