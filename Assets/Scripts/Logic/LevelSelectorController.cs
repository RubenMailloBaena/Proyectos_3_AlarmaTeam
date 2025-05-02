using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorController : MonoBehaviour
{
    private void Awake()
    {
        if (GameManager.GetInstance() == null)
            ShowMessage();
    }
    public void StartGame(int levelNumber)
    {
        LevelChangeManager.GetInstance().StartGame(levelNumber);
    }

    public void ReturnToMainMenu()
    {
        LevelChangeManager.GetInstance().GoToMainMenu();
    }

    private void ShowMessage()
    {
        //POR SI EMPEZAMOS DESDE EL MAIN MENU
        Debug.LogError("------- SHOULD START FROM BOOT SCENE! --------");
        SceneManager.LoadScene("-BootScene-");
    }
}
