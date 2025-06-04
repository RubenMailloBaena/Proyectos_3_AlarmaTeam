using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private ScriptableRendererFeature fullScreenVision;
    
    private void Awake()
    {
        if(GameManager.GetInstance() == null)
            ShowMessage();
        
        fullScreenVision.SetActive(false);
    }

    public void StartGame(int levelNumber)
    {
        LevelChangeManager.GetInstance().StartGame(levelNumber);
    }

    public void ExitGame()
    {
        GameManager.GetInstance().ExitGame();
    }

    public void GoToLevelSelector()
    {
        LevelChangeManager.GetInstance().GoToLevelSelector();
    }

    private void ShowMessage()
    {
        //POR SI EMPEZAMOS DESDE EL MAIN MENU
        Debug.LogError("------- SHOULD START FROM BOOT SCENE! --------");
        SceneManager.LoadScene("-BootScene-");
    }
}
