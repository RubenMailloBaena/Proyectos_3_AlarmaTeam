using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform pitchController;
    [SerializeField] private float secondsThatCanTakeDamage = 5f;
    [SerializeField] private float cooldownToHealPlayer = 2f;
    private float currentHealth;
    private float timeTillLastDamage = 0.0f;
    
    public bool IsCrouching { get; private set; }
    public bool IsLeaning { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsVaulting { get; private set; }
    public bool IsTeleporting { get; private set; }
    public bool IsUsingVision { get; private set; }
    public bool IsPlayerDead { get; private set; }

    public event Action<bool> OnCameraLockChange;
    public event Action OnVaultCrouched;
    
    private void Awake()
    {
        GameManager.GetInstance().SetPlayerController(this);
        currentHealth = secondsThatCanTakeDamage;
    }

    private void Update()
    {
        HealPlayer();
    }

    public void LockCamera(bool lockCam)
    {
        OnCameraLockChange?.Invoke(lockCam);
    }

    public void TryVaultCrouched()
    {
        OnVaultCrouched?.Invoke();
    }

    public bool TakeDamage()
    {
        currentHealth -= Time.deltaTime;
        timeTillLastDamage = cooldownToHealPlayer;

        return currentHealth <= 0.0f;
    }

    private void HealPlayer()
    {
        if (timeTillLastDamage <= 0.0f)
        {
            currentHealth += Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, secondsThatCanTakeDamage);
        }
        else
            timeTillLastDamage -= Time.deltaTime;
        print("PlayerHealth: " + currentHealth);
    }

    public Vector3 GetPlayerEyesPosition() => pitchController.position;
    public Vector3 GetPlayerPosition() => transform.position;
    public void SetLeaning(bool leaning) => IsLeaning = leaning;
    public void SetCrouching(bool crouching) => IsCrouching = crouching;
    public void SetGrounded(bool grounded) => IsGrounded = grounded;
    public void SetVaulting(bool vaulting) => IsVaulting = vaulting;
    public void SetTeleporting(bool teleporting) => IsTeleporting = teleporting;
    public void SetUsingVision(bool vision) => IsUsingVision = vision;
    public void SetIsPlayerDead(bool dead) => IsPlayerDead = dead;
}
