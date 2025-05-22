using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : State
{
    [Header("PATROL")]
    public float patrolSpeed = 3f;
    private Vector3 targetPos;
    
    [Header("STATES")]
    public IdleState idleState;

    public override void InitializeState()
    {
        eController.SetAnimation(AnimationType.Walk, false);
        EnemyAudioManager.SetSound(SoundType.Walk, transform.position);
        eController.SetAgentSpeed(patrolSpeed);

        
    }

    public override State RunCurrentState()
    {
        targetPos = eController.GoToWaypoint();
        if (eController.ArrivedToPosition(targetPos))
            return idleState;
        return this;
    }
}
