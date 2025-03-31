using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private PlayerMovementController playerMovement;
    private PlayerLookController playerLookController;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #region Getters & Setters
    public static GameManager GetInstance() => instance;
    public PlayerLookController GetPlayerLookController() => playerLookController;
    public void SetPlayerLookController(PlayerLookController movementController) => playerLookController = movementController;
    public PlayerMovementController GetPlayerMovement() => playerMovement;
    public void SetPlayerMovement(PlayerMovementController controller) => playerMovement = controller;
    
    #endregion


}
