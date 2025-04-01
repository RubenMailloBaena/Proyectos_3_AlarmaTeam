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
        eController.SetAgentSpeed(patrolSpeed);
        targetPos = eController.GoToWaypoint();
    }

    public override State RunCurrentState()
    {
        if (eController.ArrivedToPosition(targetPos))
            return idleState;
        return this;
    }
}
