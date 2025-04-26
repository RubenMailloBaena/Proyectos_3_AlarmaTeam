using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckpointAdapter : MonoBehaviour
{
    [SerializeField] private EnemyController enemy;

    public bool isEnemyDead()
    {
        return enemy.enemyIsDead;
    }
}
