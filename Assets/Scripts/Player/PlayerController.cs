using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IRestartable
{
    private bool movedPos;
    
    // ------------------ REFERENCIAS ------------------

    private PlayerHUDController pHUD;
    private PlayerInput playerInput;

    [SerializeField] private Transform playerEyes;
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform playerBody;

    private IPlayerComponent HUDActivator; // Componente que activó el texto de inputs
    
    private HashSet<IEnemyHear> hearEnemies = new HashSet<IEnemyHear>();
    private HashSet<IEnemyBackstab> backstabsEnemies = new HashSet<IEnemyBackstab>();
    private HashSet<IVisible> visionObjects = new HashSet<IVisible>();
    
    // ----------- RESTART ATTRIBUTES --------

    private Vector3 startingPos, checkpointPos;
    private Quaternion startingRotation, checkpointRotation;
    
    // ------------------ ESTADOS ------------------

    public bool IsCrouching { get; private set; }
    public bool IsIdle { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsLeaning { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsVaulting { get; private set; }
    public bool CanVault { get; private set; }
    public bool IsTeleporting { get; private set; }
    public bool IsUsingVision { get; private set; }
    public bool IsPlayerDead { get; private set; }

    public event Action<bool> OnCameraLockChange;
    public event Action onGodMode;
    public event Action OnVaultCrouched;
    public event Action<float> OnTakeDamage;
    public event Action onRestart;
    public event Action onRestartFromCheckpoint;
    public event Action onSetCheckpoint;
    public event Action OnPlayerTP;


    private void Awake()
    {
        if (GameManager.GetInstance().GetPlayerController() == null)
        {
            GameManager.GetInstance().SetPlayerController(this);
            GameManager.GetInstance().AddRestartable(this);
            SetPosition(transform);
        }
        else
        {
            GameManager.GetInstance().GetPlayerController().ChangePlayerPos(transform);
            Destroy(gameObject);
        }
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        pHUD = GameManager.GetInstance().GetPlayerHUD();
    }

    private void ChangePlayerPos(Transform targetPos)
    {
        SetPosition(targetPos);
        
        if (movedPos) return;
        movedPos = true;
        transform.position = targetPos.position;
        transform.rotation = targetPos.rotation;
    }

    private void SetPosition(Transform targetPos)
    {
        startingPos = targetPos.position;
        checkpointPos = startingPos;
        startingRotation = targetPos.rotation;
        checkpointRotation = startingRotation;
    }

    // ------------------ SETTERS ESTADOS ------------------

    public void SetLeaning(bool leaning) => IsLeaning = leaning;
    public void SetCrouching(bool crouching) => IsCrouching = crouching;
    public void SetGrounded(bool grounded) => IsGrounded = grounded;
    public void SetVaulting(bool vaulting) => IsVaulting = vaulting;
    public void SetCanVault(bool vaulting) => CanVault = vaulting;
    public void SetTeleporting(bool teleporting) => IsTeleporting = teleporting;
    public void SetUsingVision(bool vision) => IsUsingVision = vision;
    public void SetIsPlayerDead(bool dead) => IsPlayerDead = dead;
    public void SetIsIdle(bool idle) => IsIdle = idle;
    public void SetIsRunning(bool running) => IsRunning = running;

    // ------------------ EVENTOS ------------------
    public void LockCamera(bool lockCam) => OnCameraLockChange?.Invoke(lockCam);
    public void TryVaultCrouched() => OnVaultCrouched?.Invoke();
    public float GetDistance(Vector3 targetPos) => Vector3.Distance(transform.position, targetPos);
    public void TakeDamage(float damage) => OnTakeDamage?.Invoke(damage);
    public void SwapGodMode() => onGodMode?.Invoke();
    public void PlayerTP() => OnPlayerTP?.Invoke();

    // ------------------ HUD ------------------

    public void CanInteract(InputAction input, InputType inputType, IPlayerComponent playerComponent)
    {
        if (HUDActivator != null) return;
        HUDActivator = playerComponent;
        pHUD.SetInteractionText(input, inputType);
    }

    public void HideInteract(IPlayerComponent playerComponent)
    {
        if (playerComponent != HUDActivator) return;
        HUDActivator = null;
        pHUD.HideInteract();
    }

    public void UpdateProgressBar(float progress) => pHUD.UpdateProgressBar(progress);
    public void HideProgressBar() => pHUD.HideProgressBar();
    public void SetCharmingVisual(bool active) => pHUD.SetCharmingVisualActive(active);
    public void SetHurtVisualColor(Color color) => pHUD.SetHurtVisualColor(color);
    public Color GetHurtVisualColor() => pHUD.GetHurtVisualColor();
    public void ShowGameOverHUD(bool b) => pHUD.SetGameOverPanelActive(b);

    // ------------------ PLAYER REFERENCES ------------------

    public Vector3 GetPlayerEyesPosition() => playerEyes.position;
    public Vector3 GetPlayerHeadPosition() => playerHead.position;
    public Vector3 GetPlayerBodyPosition() => playerBody.position;
    public Vector3 GetPlayerPosition() => transform.position;
    public Transform GetPlayerTransform() => transform;
    public PlayerInput GetPlayerInput() => playerInput;

    // ------------------ ENEMIGOS Y VISION ------------------

    public void AddVisible(IVisible visible) => visionObjects.Add(visible);
    public void RemoveVisible(IVisible visible) => visionObjects.Remove(visible);
    public HashSet<IVisible> GetVisionObjects() => visionObjects;

    public void AddHearEnemy(IEnemyHear enemy) => hearEnemies.Add(enemy);
    public void RemoveHearEnemy(IEnemyHear enemy) => hearEnemies.Remove(enemy);
    public HashSet<IEnemyHear> GetHearEnemies() => hearEnemies;
    
    public void AddBackstabEnemy(IEnemyBackstab enemy) => backstabsEnemies.Add(enemy);
    public void RemoveBackstabEnemy(IEnemyBackstab enemy) => backstabsEnemies.Remove(enemy);
    public HashSet<IEnemyBackstab> GetBackstabEnemies() => backstabsEnemies;
    
    
    // ----------------- CHECKPOINTS ------------------
    public void RestartGame()
    {
        transform.position = startingPos;
        transform.rotation = startingRotation;
        checkpointPos = startingPos;
        checkpointRotation = startingRotation;
        onRestart?.Invoke();
    }

    public void RestartFromCheckPoint()
    {
        transform.position = checkpointPos;
        transform.rotation = checkpointRotation;
        onRestartFromCheckpoint?.Invoke();
    }

    public void SetCheckPoint()
    {
        checkpointPos = transform.position;
        checkpointRotation = transform.rotation;
        onSetCheckpoint?.Invoke();
    }

    private void OnDestroy() => GameManager.GetInstance().RemoveRestartable(this);
}
