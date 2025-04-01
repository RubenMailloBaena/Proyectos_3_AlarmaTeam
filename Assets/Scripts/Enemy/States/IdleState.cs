using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    private float waitTime = 0.0f;

    [Header("STATES")]
    public PatrolState patrolState;

    private void Start()
    {
        if(!eController.isIdleEnemy) eController.SwitchToNextState(patrolState);
    }

    public override void InitializeState()
    {
        waitTime = eController.GetWaitTime();
    }

    public override State RunCurrentState()
    {
        if(eController.isIdleEnemy) return this;

        waitTime -= Time.deltaTime;

        if (waitTime <= 0.0f)
        {
            eController.IncrementIndex();
            return patrolState;
        }
        
        return this;
    }
}
