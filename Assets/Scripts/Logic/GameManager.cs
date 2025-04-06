using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private RawImage PlayerHurtPanel;
    [SerializeField] private RawImage PlayerCharmingImage;

    private PlayerController pController;
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
    public RawImage GetPlayerHurtHud() => PlayerHurtPanel;
    public RawImage GetPlayerCharmingImage() => PlayerCharmingImage;
    public static GameManager GetInstance() => instance;
    public PlayerController GetPlayerController() => pController;
    public void SetPlayerController(PlayerController info) => pController = info;
    #endregion
}
