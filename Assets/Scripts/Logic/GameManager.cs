using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    private PlayerController pController;
    private PlayerHUDController pHud;
    private EnemySeenHUD eHud;
    private HashSet<IInteractable> levers = new HashSet<IInteractable>();
    private OnlyOneInstance directionalLight;
    private OnlyOneInstance eventSystem;

    private List<IRestartable> restartObjects = new List<IRestartable>();

    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadSceneAsync("Level2-Test", LoadSceneMode.Additive);
        }
    }

    public void PlayerDead()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void RestartGame()
    {
        RestartGameOverValues();
        foreach (IRestartable restartable in restartObjects)
        {
            restartable.RestartGame();
        }
    }

    public void RestartFromCheckpoint()
    {
        RestartGameOverValues();
        foreach (IRestartable restartable in restartObjects)
        {
            restartable.RestartFromCheckPoint();
        }
    }
    
    private void RestartGameOverValues()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetCheckpoint()
    {
        foreach (IRestartable restartable in restartObjects)
        {
            restartable.SetCheckPoint();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    #region Getters & Setters
    public void AddRestartable(IRestartable restartable) => restartObjects.Add(restartable);
    public void AddInteractable(IInteractable interactable) => levers.Add(interactable);
    public HashSet<IInteractable> GetInteractables() => levers;
    public static GameManager GetInstance() => instance;
    public PlayerController GetPlayerController() => pController;
    public void SetPlayerController(PlayerController info) => pController = info;
    public PlayerHUDController GetPlayerHUD() => pHud;
    public void SetPlayerHUD(PlayerHUDController pHud) => this.pHud = pHud;
    public EnemySeenHUD GetEnemySeenHUD() => eHud;
    public void SetEnemySeenHUD(EnemySeenHUD hud) => eHud = hud;
    public void SetLight(OnlyOneInstance light) => directionalLight = light;
    public bool LevelHasNoLight() => directionalLight == null;
    public void SetEventSystem(OnlyOneInstance eventS) => eventSystem = eventS;
    public bool LevelHasNoEventSystem() => eventSystem == null;

    #endregion
}
