using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum InputType {
    Press,
    Hold
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
    [SerializeField] private TextMeshProUGUI UIText;
    [SerializeField] private InputActionReference testInput;
    private PlayerInput playerInput;

    [Header("Teleport Bar")]
    [SerializeField] private GameObject UIFatherBar;
    [SerializeField] private RawImage UIProgressBar;
    
    [Header("Player Hurt Visuals")]
    [SerializeField] private RawImage playerHurtPanel;
    
    [Header("Player Charm Visuals")]
    [SerializeField] private RawImage playerCharmingImage;

    [Header("GameLogic UI")] 
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        GameManager.GetInstance().SetPlayerHUD(this);
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

    public void SetInteractionText(InputAction input, InputType type)
    {
        string currentControl = playerInput.currentControlScheme;
        string bindingPath = "";
        
        if(currentControl.Equals("Keyboard&Mouse"))
            bindingPath = input.bindings[0].effectivePath;
        else
            bindingPath = input.bindings[1].effectivePath;

        string displayString = InputControlPath.ToHumanReadableString(bindingPath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        string result = "";
        result = type == InputType.Press ? "Press '" : "Hold '";
        
        result += displayString + "' to interact";
        UIText.enabled = true;
        UIText.text = result;
    }

    public void HideInteract()
    {
        UIText.enabled = false;
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
        restartButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        
        restartButton.onClick.AddListener(GameManager.GetInstance().RestartScene);
        exitButton.onClick.AddListener(GameManager.GetInstance().ExitGame);
    }
    public void SetGameOverPanelActive(bool active) => gameOverPanel.SetActive(active);
    #endregion
}
