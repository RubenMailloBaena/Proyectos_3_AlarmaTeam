using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    [SerializeField] private GameObject GameOverPanel;

    private PlayerController pController;
    private PlayerHUDController pHud;
    private EnemySeenHUD eHud;
    private HashSet<IInteractable> levers = new HashSet<IInteractable>();
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlayerDead()
    {
        GameOverPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
