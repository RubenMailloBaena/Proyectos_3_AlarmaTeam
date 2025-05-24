using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBackstab : MonoBehaviour, IEnemyBackstab
{
    private EnemyController eController;
    [SerializeField] private GameObject weakSpotRenderer;
    
    public void SetBackstab(EnemyController enemyController)
    {
        eController = enemyController;
    }
    
    private void Start()
    {
        GameManager.GetInstance().GetPlayerController().AddBackstabEnemy(this);
    }

    public void SetWeakSpot(bool active)
    {
        if (eController.enemyIsDead) return;
        
        if(weakSpotRenderer != null)
            weakSpotRenderer.SetActive(active);
    }

    public void KillAnimationHeartController(bool active)
    {
        if(weakSpotRenderer != null)
            weakSpotRenderer.SetActive(active);
    }

    public void Backstab()
    {
        eController.SwitchToDieState();
    }

    public Transform GetTransform() => transform;
    public bool IsEnemyDead() => eController.enemyIsDead;

    private void OnDestroy()
    {
        GameManager.GetInstance().GetPlayerController().RemoveBackstabEnemy(this);
    }
}
