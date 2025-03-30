using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private PlayerController playerController;
    private CheatUI debugCheats;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #region Getters & Setters
    public static GameManager GetInstance() => instance;
    public PlayerController GetPlayerController() => playerController;
    public void SetPlayerController(PlayerController controller) => playerController = controller;
    public void SetDebugCheats(CheatUI cheats) => debugCheats = cheats;
    public CheatUI GetDebugCheats() => debugCheats;
    #endregion


}
