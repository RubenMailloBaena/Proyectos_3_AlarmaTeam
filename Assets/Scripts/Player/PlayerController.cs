using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform playerEyes;
    public bool IsCrouching { get; private set; }
    public bool IsLeaning { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsVaulting { get; private set; }
    public bool IsTeleporting { get; private set; }
    public bool IsUsingVision { get; private set; }
    public bool IsPlayerDead { get; private set; }
    public HashSet<IEnemyInteractions> enemies = new HashSet<IEnemyInteractions>();
    public HashSet<IVisible> visionObjects = new HashSet<IVisible>();
    public event Action<bool> OnCameraLockChange;
    public event Action OnVaultCrouched;
    public event Action OnTakeDamage;
    
    private void Awake()
    {
        GameManager.GetInstance().SetPlayerController(this);
    }

    public float GetDistance(Vector3 targetPos) => Vector3.Distance(transform.position, targetPos);
    public void LockCamera(bool lockCam) => OnCameraLockChange?.Invoke(lockCam);
    public void TryVaultCrouched() => OnVaultCrouched?.Invoke();
    public void TakeDamage() => OnTakeDamage?.Invoke();
    
    public Vector3 GetPlayerEyesPosition() => playerEyes.position;
    public Vector3 GetPlayerPosition() => transform.position;
    public Transform GetPlayerTransform() => transform;
    
    public void AddVisible(IVisible visible) => visionObjects.Add(visible);
    public void RemoveVisible(IVisible visible) => visionObjects.Remove(visible);
    public HashSet<IVisible> GetVisionObjects() => visionObjects;
    public void AddEnemy(IEnemyInteractions enemy) => enemies.Add(enemy);
    public void RemoveEnemy(IEnemyInteractions enemy) => enemies.Remove(enemy);
    public HashSet<IEnemyInteractions> GetEnemies() => enemies;
    public void SetLeaning(bool leaning) => IsLeaning = leaning;
    public void SetCrouching(bool crouching) => IsCrouching = crouching;
    public void SetGrounded(bool grounded) => IsGrounded = grounded;
    public void SetVaulting(bool vaulting) => IsVaulting = vaulting;
    public void SetTeleporting(bool teleporting) => IsTeleporting = teleporting;
    public void SetUsingVision(bool vision) => IsUsingVision = vision;
    public void SetIsPlayerDead(bool dead) => IsPlayerDead = dead;
}
