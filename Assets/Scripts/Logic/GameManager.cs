using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    //REFERENCES
    private PlayerController pController;
    private PlayerHUDController pHud;
    private EnemySeenHUD eHud;
    
    //INSTANCES
    private HashSet<IInteractable> levers = new HashSet<IInteractable>();
    private OnlyOneInstance directionalLight;
    private OnlyOneInstance eventSystem;
    
    //LEVEL LOGIC
    private List<IRestartable> restartObjects = new List<IRestartable>();
    
    //WIN CONDITION
    private int enemiesAlive = 0;
    private LevelDoor finalDoor;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    
    //DEBUG
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadSceneAsync("Level2-Test", LoadSceneMode.Additive);
        }
    }

    #region WinCondition
    public void AddEnemyAlive() => enemiesAlive++;
    public void RemoveEnemieAlive()
    {
        enemiesAlive--;
        if(enemiesAlive <= 0 && finalDoor != null)
            finalDoor.UnlockDoor();
    } 
    
    public void SetFinalDoor(LevelDoor finalDoor)
    {
        this.finalDoor = finalDoor;
        finalDoor.LockDoor();
    }

    public void RemoveFinalDoor() => finalDoor = null;

    #endregion

    #region Level Logic
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
        eHud.HideAllArrows();
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
    #endregion
    
    #region Getters & Setters
    public void AddRestartable(IRestartable restartable) => restartObjects.Add(restartable);
    public void RemoveRestartable(IRestartable restartable) => restartObjects.Remove(restartable);
    public void AddInteractable(IInteractable interactable) => levers.Add(interactable);
    public void RemoveInteractable(IInteractable interactable) => levers.Remove(interactable);
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
