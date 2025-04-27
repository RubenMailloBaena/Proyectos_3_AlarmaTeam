using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckpointAdapter : MonoBehaviour
{
    private Checkpoint levelCheckpoin;

    public void SetEnemyAdapter(Checkpoint checkpoint)
    {
        levelCheckpoin = checkpoint;
    }

    public void EnemyDead()
    {
        if (levelCheckpoin != null)
            levelCheckpoin.EnemyDead();
    }

    public void EnemyRespawn()
    {
        if (levelCheckpoin != null)
            levelCheckpoin.EnemyRespawn();
    }
}
