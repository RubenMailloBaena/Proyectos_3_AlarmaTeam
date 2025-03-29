using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private PlayerController playerController;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    void Update()
    {
        DebugOptions();
    }

    private void DebugOptions()
    {
        if (Input.GetKeyDown(KeyCode.O))
            playerController.LockCursor();

        if (Input.GetKeyDown(KeyCode.P))
            playerController.UnlockCursor();
    }

    #region Getters & Setters
    public static GameManager GetInstance() => instance;

    public PlayerController GetPlayerController() => playerController;

    public void SetPlayerController(PlayerController controller) => playerController = controller;
    #endregion


}
