using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
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
    
    [Header("Player Crouch Visuals")]
    [SerializeField] private RawImage playerCrouchVisual;

    [Header("Player Charm Visuals")] 
    [SerializeField] private Material vampVisionMat;
    [SerializeField] private ScriptableRendererFeature fullScreenVision;
    [SerializeField] private float targetScreenIntesity = 0.6f;
    [SerializeField] private float targetVinegget = 1.2f;
    private Coroutine shaderLerpCoroutine;
    
    private float vignettIntesity, fullScreenIntensity;
    
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

    [SerializeField] private GameObject panelOptions;

    [Header("FINAL ANIMATION")] [SerializeField]
    private Animator finalScene;

    private Bus masterBus;


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

        //LERP MATERIAL
        vignettIntesity = vampVisionMat.GetFloat("_VignettIntesity");
        fullScreenIntensity = vampVisionMat.GetFloat("_FullScreenIntensity");

        masterBus = RuntimeManager.GetBus("bus:/");
    }
    
    public void ShowCrossHair(bool show)
    {
        crosshair.SetActive(show);
    }

    public void StartCuttScene()
    {
        finalScene.SetTrigger("StartCutscene");
    }

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


    public void SetCharmingVisualActive(bool active)
    {
        if (shaderLerpCoroutine != null)
            StopCoroutine(shaderLerpCoroutine);

        shaderLerpCoroutine = StartCoroutine(LerpShaderCoroutine(active));
    }
    
    private IEnumerator LerpShaderCoroutine(bool active)
    {
        fullScreenVision.SetActive(true);

        float effectDuration = pController.GetCharmEffectDuration();
        float elapsed = 0f;

        float startVignette = vampVisionMat.GetFloat("_VignettIntesity");
        float startFullScreen = vampVisionMat.GetFloat("_FullScreenIntensity");

        float targetVignette = active ? targetVinegget : 0f;
        float targetFullScreen = active ? targetScreenIntesity : 1f;

        while (elapsed < effectDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / effectDuration;

            float currentVignette = Mathf.Lerp(startVignette, targetVignette, t);
            float currentFullScreen = Mathf.Lerp(startFullScreen, targetFullScreen, t);

            vampVisionMat.SetFloat("_VignettIntesity", currentVignette);
            vampVisionMat.SetFloat("_FullScreenIntensity", currentFullScreen);

            yield return null;
        }

        vampVisionMat.SetFloat("_VignettIntesity", targetVignette);
        vampVisionMat.SetFloat("_FullScreenIntensity", targetFullScreen);
        
        fullScreenVision.SetActive(active);
    }

    #endregion
    
    #region PlayerHurtVisual
    public void SetHurtVisualColor(Color color) => playerHurtPanel.color = color;
    public Color GetHurtVisualColor() => playerHurtPanel.color;

    #endregion

    #region PlayerCrouchVisual
    public void SetCrouchVisualColor(float alphaValue)
    {
        Color color = playerCrouchVisual.color;
        color.a = alphaValue;
        playerCrouchVisual.color = color;
    }
    public float GetCrouchAlphaValue() => playerCrouchVisual.color.a;
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
        if (!active)
        {
            panelOptions.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = active;
        
        if (active)
        {
            Time.timeScale = 0f;
            PauseAllSounds();
        }
        else
        {
            Time.timeScale = 1.0f;
            ResumeAllSounds();
        }
    }

    public void PauseAllSounds()
    {
        masterBus.setPaused(true);
    }

    public void ResumeAllSounds()
    {
        masterBus.setPaused(false);
    }

    public void RestartFromPauseMenu()
    {
        SetPauseMenu(false);
        GameManager.GetInstance().RestartGame();
    }
    public void GoToOptionsMenu()
    {
        if (pausePanel != null && panelOptions != null)
        {
            pausePanel.SetActive(false);
            panelOptions.SetActive(true);
        }
        else
        {
            Debug.LogError("Paneles no asignados en el Inspector!");
        }
    }
    public void RestartFromOptionsMenu()
    {
        panelOptions.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void DisableRestartLevel(bool active) => pauseRestartButton.interactable = !active;
    #endregion
}
