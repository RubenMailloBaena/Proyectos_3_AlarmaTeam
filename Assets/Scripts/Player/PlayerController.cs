using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsCrouching { get; private set; }
    public bool IsLeaning { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsVaulting { get; private set; }
    public bool IsTeleporting { get; private set; }

    public event Action<bool> OnCameraLockChange;
    public event Action OnVaultCrouched;
    
    private void Awake()
    {
        GameManager.GetInstance().SetPlayerController(this);
    }

    public void LockCamera(bool lockCam)
    {
        OnCameraLockChange?.Invoke(lockCam);
    }

    public void TryVaultCrouched()
    {
        OnVaultCrouched?.Invoke();
    }
    
    public void SetLeaning(bool leaning) => IsLeaning = leaning;
    public void SetCrouching(bool crouching) => IsCrouching = crouching;
    public void SetGrounded(bool grounded) => IsGrounded = grounded;
    public void SetVaulting(bool vaulting) => IsVaulting = vaulting;
    public void SetTeleporting(bool teleporting) => IsTeleporting = teleporting;
}
