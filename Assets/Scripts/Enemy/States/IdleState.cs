using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    [Header("IDLE")]
    public float idleTime = 3f;
    private float currentTime;

    [Header("STATES")]
    public PatrolState patrolState;

    public override void InitializeState()
    {
        currentTime = 0.0f;
    }

    public override State RunCurrentState()
    {
        return this;
    }
}
