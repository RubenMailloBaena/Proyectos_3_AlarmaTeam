using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private SceneField startButtonScene;
    private GameManager gm;
    
    private void Awake()
    {
        if (GameManager.GetInstance() == null)
        {
            Debug.LogError("----------- YOU MUST ENTER FROM BOOT SCENE! --------------");
            return;
        }
        gm = GameManager.GetInstance();
    }

    public void StartGame()
    {
        gm.StartGame(startButtonScene);
    }

    public void ExitGame()
    {
        gm.ExitGame();
    }
}
