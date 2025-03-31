using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsCrouching { get; private set; }
    public bool IsLeaning { get; private set; }
    public bool IsGrounded { get; private set; }

    public event Action<bool> OnCameraLockChange; 
    
    private void Awake()
    {
        GameManager.GetInstance().SetPlayerController(this);
    }

    public void LockCamera(bool lockCam)
    {
        OnCameraLockChange?.Invoke(lockCam);
    }
    
    public void SetLeaning(bool leaning) => IsLeaning = leaning;
    public void SetCrouching(bool crouching) => IsCrouching = crouching;
    public void SetGrounded(bool grounded) => IsGrounded = grounded;
}
