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
        return enemiesInZone.All(enemy => enemy.isEnemyDead());
    }

    private void SetCheckpoint()
    {
        print("CHECKPOINT SAVED");
        checkpointSeted = true;
        GameManager.GetInstance().GetPlayerController().SetPlayerRespawnPos(playerRespawnPlayer);
        GameManager.GetInstance().SetCheckpoint();
    }
    
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
