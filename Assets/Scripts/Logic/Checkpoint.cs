using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform playerRespawnPlayer;
    [SerializeField] private List<EnemyController> enemiesInZone;
    private bool isPlayerInside, checkpointSeted;

    private void Start()
    {
        if(enemiesInZone.Count == 0)
            Debug.LogError("------- CHECK POINT WITH NO ENEMIES IN THE LIST: " + gameObject.name + " ---------------");
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
        return enemiesInZone.All(enemy => enemy.enemyIsDead);
    }

    private void SetCheckpoint()
    {
        Debug.LogWarning("CHECKPOINT SETTED");
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
