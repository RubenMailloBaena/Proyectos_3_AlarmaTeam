using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // ------------------ REFERENCIAS ------------------

    private PlayerHUDController pHUD;
    private PlayerInput playerInput;

    [SerializeField] private Transform playerEyes;
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform playerBody;

    private IPlayerComponent HUDActivator; // Componente que activó el texto de inputs

    // ------------------ ESTADOS ------------------

    public bool IsCrouching { get; private set; }
    public bool IsIdle { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsLeaning { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsVaulting { get; private set; }
    public bool IsTeleporting { get; private set; }
    public bool IsUsingVision { get; private set; }
    public bool IsPlayerDead { get; private set; }

    public event Action<bool> OnCameraLockChange;
    public event Action onGodMode;
    public event Action OnVaultCrouched;
    public event Action<float> OnTakeDamage;


    private void Awake()
    {
        GameManager.GetInstance().SetPlayerController(this);
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        pHUD = GameManager.GetInstance().GetPlayerHUD();
    }

    // ------------------ SETTERS ESTADOS ------------------

    public void SetLeaning(bool leaning) => IsLeaning = leaning;
    public void SetCrouching(bool crouching) => IsCrouching = crouching;
    public void SetGrounded(bool grounded) => IsGrounded = grounded;
    public void SetVaulting(bool vaulting) => IsVaulting = vaulting;
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

    // ------------------ PLAYER REFERENCES ------------------

    public Vector3 GetPlayerEyesPosition() => playerEyes.position;
    public Vector3 GetPlayerHeadPosition() => playerHead.position;
    public Vector3 GetPlayerBodyPosition() => playerBody.position;
    public Vector3 GetPlayerPosition() => transform.position;
    public Transform GetPlayerTransform() => transform;
    public PlayerInput GetPlayerInput() => playerInput;

    // ------------------ ENEMIGOS Y VISION ------------------

    private HashSet<IEnemyInteractions> enemies = new HashSet<IEnemyInteractions>();
    private HashSet<IVisible> visionObjects = new HashSet<IVisible>();

    public void AddVisible(IVisible visible) => visionObjects.Add(visible);
    public void RemoveVisible(IVisible visible) => visionObjects.Remove(visible);
    public HashSet<IVisible> GetVisionObjects() => visionObjects;

    public void AddEnemy(IEnemyInteractions enemy) => enemies.Add(enemy);
    public void RemoveEnemy(IEnemyInteractions enemy) => enemies.Remove(enemy);
    public HashSet<IEnemyInteractions> GetEnemies() => enemies;
}
