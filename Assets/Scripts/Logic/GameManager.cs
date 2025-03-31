using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private PlayerController pController;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #region Getters & Setters
    public static GameManager GetInstance() => instance;
    public PlayerController GetPlayerController() => pController;
    public void SetPlayerController(PlayerController info) => pController = info;
    #endregion
}
