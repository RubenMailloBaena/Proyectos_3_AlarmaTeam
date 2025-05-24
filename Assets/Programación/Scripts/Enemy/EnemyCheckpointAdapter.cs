using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckpointAdapter : MonoBehaviour
{
    private Checkpoint levelCheckpoin;

    private void Start() => GameManager.GetInstance().AddEnemyAlive();

    public void SetEnemyAdapter(Checkpoint checkpoint)
    {
        levelCheckpoin = checkpoint;
    }

    public void EnemyDead()
    {
        GameManager.GetInstance().RemoveEnemieAlive();
        if (levelCheckpoin != null)
            levelCheckpoin.EnemyDead();
    }

    public void EnemyRespawn()
    {
        GameManager.GetInstance().AddEnemyAlive();
        if (levelCheckpoin != null)
            levelCheckpoin.EnemyRespawn();
    }
}
