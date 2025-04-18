using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBackstab : MonoBehaviour, IEnemyBackstab
{
    [SerializeField] private GameObject weakSpotRenderer;

    private void Start()
    {
        GameManager.GetInstance().GetPlayerController().AddBackstabEnemy(this);
    }

    public void SetWeakSpot(bool active)
    {
        weakSpotRenderer.SetActive(active);
    }

    public void Backstab()
    {
        Destroy(gameObject);
    }

    public Transform GetTransform() => transform;

    private void OnDestroy()
    {
        GameManager.GetInstance().GetPlayerController().RemoveBackstabEnemy(this);
    }
}
