using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform playerRespawnPlayer;
    [SerializeField] private List<EnemyCheckpointAdapter> enemiesInZone = new List<EnemyCheckpointAdapter>();
    private bool isPlayerInside, checkpointSeted;
    private int currentAliveEnemies;

    private void Awake()
    {
        if (enemiesInZone.Count != 0)
            foreach (EnemyCheckpointAdapter enemy in enemiesInZone)
                enemy.SetEnemyAdapter(this);

        currentAliveEnemies = enemiesInZone.Count;
    }

    private void Update()
    {
        CheckpointLogic();
    }

    private void CheckpointLogic()
    {
        if (!isPlayerInside || checkpointSeted) return;

        if (IsSafeZone())
            SetCheckpoint();
    }

    private bool IsSafeZone()
    {
        return currentAliveEnemies <= 0;
    }

    private void SetCheckpoint()
    {
        print("CHECKPOINT SAVED");
        checkpointSeted = true;
        GameManager.GetInstance().GetPlayerController().SetPlayerRespawnPos(playerRespawnPlayer);
        GameManager.GetInstance().SetCheckpoint();
    }

    public void EnemyDead() => currentAliveEnemies--;
    public void EnemyRespawn() => currentAliveEnemies++;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            checkpointSeted = false;
        }
    }
}
