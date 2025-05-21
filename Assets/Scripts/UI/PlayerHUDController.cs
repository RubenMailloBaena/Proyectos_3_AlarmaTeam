using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum InputType {
    Press,
    Hold
}

public enum ActionType {
    Default,
    Vault,
    Teleport,
    Backstab,
    Crouch
}

public class PlayerHUDController : MonoBehaviour
{
    private PlayerController pController;
    
    [Header("Player Sound")]
    [SerializeField] private Texture sound1;
    [SerializeField] private Texture sound2;
    [SerializeField] private Texture sound3;
    [SerializeField] private RawImage UIImage;

    [Header("Interactions Text")] 
    [SerializeField] private TextMeshProUGUI UIinputTypeText;
    [SerializeField] private TextMeshProUGUI UIactionTypeText;
    [SerializeField] private Image UIIconSprite;
    [SerializeField] private InputActionReference testInput;
    private PlayerInput playerInput;
    private InteractionIconsRepo iconsRepo;

    [Header("Teleport Bar")]
    [SerializeField] private GameObject UIFatherBar;
    [SerializeField] private RawImage UIProgressBar;
    
    [Header("Player Hurt Visuals")]
    [SerializeField] private RawImage playerHurtPanel;
    
    [Header("Player Charm Visuals")]
    [SerializeField] private RawImage playerCharmingImage;

    [Header("GameLogic UI")] 
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button checkpointButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    
    [Space(10)]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseRestartButton;
    [SerializeField] private Button pauseExitButton;


    private void Awake()
    {
        if(GameManager.GetInstance().GetPlayerHUD() == null)
            GameManager.GetInstance().SetPlayerHUD(this);
        else
            Destroy(transform.parent.gameObject);

        iconsRepo = GetComponent<InteractionIconsRepo>();
    }

    private void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
        playerInput = pController.GetPlayerInput();
        SetButtons();
    }
    
    private void Update()
    {
        HandleSoundVisuals();
    }

    public void ShowCrossHair(bool show)
    {
        crosshair.SetActive(show);
    }

    #region Sound Functions

    private void HandleSoundVisuals()
    {
        if (!pController.IsIdle)
        {
            UIImage.enabled = true;
            UIImage.texture = GetSoundTexture();
        }
        else
            UIImage.enabled = false;
    }

    private Texture GetSoundTexture()
    {
        if (pController.IsCrouching) return sound1;
        if (!pController.IsRunning) return sound2;
        return sound3;
    }

    #endregion

    #region InteractionTexts

    public void SetInteractionText(InputAction input, InputType type, ActionType actionType)
    {
        KeySpritePair display = iconsRepo.GetInputText(input, type, actionType, playerInput);

        UIIconSprite.sprite = iconsRepo.GetCorrespondingSprite(display.keyName);
        UIinputTypeText.text = display.inputTypeText;
        UIactionTypeText.text =display.actionTypeText;
        UIinputTypeText.enabled = true;
        UIactionTypeText.enabled = true;
        UIIconSprite.enabled = true;

    }

    public void SetTutorialCrouchText(InputAction input)
    {
        UIIconSprite.sprite = iconsRepo.GetCorrespondingSprite("control");
        UIinputTypeText.text = iconsRepo.GetInputTypeString(InputType.Press);
        UIactionTypeText.text = iconsRepo.GetActionTypeString(ActionType.Crouch);
        UIinputTypeText.enabled = true;
        UIactionTypeText.enabled = true;
        UIIconSprite.enabled = true;
    }

    public void HideInteract()
    {
        UIinputTypeText.enabled = false;
        UIactionTypeText.enabled = false;
        UIIconSprite.enabled = false;
    }

    #endregion

    #region TpProgressBar

    public void UpdateProgressBar(float progress)
    {
        UIFatherBar.SetActive(true);
        float normalizedProgress = Mathf.Clamp01(progress/2f);
        Vector3 currentScale = UIProgressBar.transform.localScale;
        currentScale.x = normalizedProgress;
        UIProgressBar.transform.localScale = currentScale;
    }

    public void HideProgressBar()
    {
        UIFatherBar.SetActive(false);
    }

    #endregion

    #region CharmingVisual

    public void SetCharmingVisualActive(bool active) => playerCharmingImage.enabled = active;

    #endregion
    
    #region PlayerHurtVisual

    public void SetHurtVisualColor(Color color) => playerHurtPanel.color = color;
    public Color GetHurtVisualColor() => playerHurtPanel.color;

    #endregion

    #region GameOverVisual
    private void SetButtons()
    {
        checkpointButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        pauseExitButton.onClick.RemoveAllListeners();
        
        checkpointButton.onClick.AddListener(GameManager.GetInstance().RestartFromCheckpoint);
        restartButton.onClick.AddListener(GameManager.GetInstance().RestartGame);
        exitButton.onClick.AddListener(LevelChangeManager.GetInstance().GoToMainMenu);
        pauseExitButton.onClick.AddListener(LevelChangeManager.GetInstance().GoToMainMenu);
    }
    public void SetGameOverPanelActive(bool active) => gameOverPanel.SetActive(active);
    #endregion

    #region PauseMenu
    public void SetPauseMenu(bool active)
    {
        pausePanel.SetActive(active);
        Cursor.visible = active;
        
        if (active)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1.0f;
    }
    
    public void RestartFromPauseMenu()
    {
        SetPauseMenu(false);
        GameManager.GetInstance().RestartGame();
    }

    public void DisableRestartLevel(bool active) => pauseRestartButton.interactable = !active;
    #endregion
}
