using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    private float waitTime = 0.0f;
    private Vector3 lookDir;

    [Header("STATES")]
    public PatrolState patrolState;

    private void Start()
    {
        if(!eController.isIdleEnemy) eController.SwitchToNextState(patrolState);
    }

    public override void InitializeState()
    {
        waitTime = eController.GetWaitTime();
        lookDir = eController.GetLookDirection();
    }

    public override State RunCurrentState()
    {
        if(eController.isIdleEnemy) return this;

        if(lookDir != Vector3.zero) 
            eController.RotateEnemy(lookDir);
        
        waitTime -= Time.deltaTime;

        if (waitTime <= 0.0f)
        {
            eController.IncrementIndex();
            return patrolState;
        }
        
        return this;
    }
}
