using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    private PlayerController pController;
    private PlayerHUDController pHud;
    private EnemySeenHUD eHud;
    private HashSet<IInteractable> levers = new HashSet<IInteractable>();

    public int TestInt = -1;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void PlayerDead()
    {
        pHud.SetGameOverPanelActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    
    // SCENE MANAGMENT
    public void ChangeScene(String name)
    {
        TestInt = 2;
        SceneManager.LoadScene(name);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    #region Getters & Setters
    public void AddInteractable(IInteractable interactable) => levers.Add(interactable);
    public HashSet<IInteractable> GetInteractables() => levers;
    public static GameManager GetInstance() => instance;
    public PlayerController GetPlayerController() => pController;
    public void SetPlayerController(PlayerController info) => pController = info;
    public PlayerHUDController GetPlayerHUD() => pHud;
    public void SetPlayerHUD(PlayerHUDController pHud) => this.pHud = pHud;
    public EnemySeenHUD GetEnemySeenHUD() => eHud;
    public void SetEnemySeenHUD(EnemySeenHUD hud) => eHud = hud;
    #endregion
}
