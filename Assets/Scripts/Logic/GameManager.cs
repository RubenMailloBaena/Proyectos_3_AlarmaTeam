using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    [SerializeField] private GameObject panel;

    private PlayerController pController;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (pController.IsPlayerDead)
            panel.SetActive(true);
    }
    #region Getters & Setters
    public static GameManager GetInstance() => instance;
    public PlayerController GetPlayerController() => pController;
    public void SetPlayerController(PlayerController info) => pController = info;
    #endregion
}
