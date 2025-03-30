using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    [Header("IDLE")]
    public float idleTime = 3f;
    private float currentTime;

    public UnityEngine.AI.NavMeshAgent navMesh;

    [Header("STATES")]
    public PatrolState patrolState;

    public override void InitializeState()
    {
        Debug.Log("Enter Idle");
        currentTime = 0.0f;
    }

    public override State RunCurrentState()
    {

        // if (alertState.CanHearPlayer())
        // {
        //     return alertState;
        // }

        currentTime += Time.deltaTime;
        if (currentTime >= idleTime)
            return patrolState;

        return this;
    }
}
