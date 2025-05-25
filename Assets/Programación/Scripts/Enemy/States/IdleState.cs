using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    private float waitTime = 0.0f;
    private Vector3 lookDir;
    
    [Header("IDLE ATTRIBUTES")]
    [SerializeField] private float idleRotationSpeed = 3f;

    [Header("STATES")]
    public PatrolState patrolState;

    private void Start()
    {
        if(!eController.isIdleEnemy) eController.SwitchToNextState(patrolState);
    }

    public override void InitializeState()
    {
        eController.StopSound();
        eController.SetAnimation(AnimationType.Idle, false);
        
        waitTime = eController.GetWaitTime();
        lookDir = eController.GetLookDirection();
    }
    
    public override State RunCurrentState()
    {
        if(lookDir != Vector3.zero) 
            eController.RotateEnemy(lookDir, idleRotationSpeed);
        
        if(eController.isIdleEnemy) return this;
        
        waitTime -= Time.deltaTime;

        if (waitTime <= 0.0f)
        {
            eController.IncrementIndex();
            return patrolState;
        }
        
        return this;
    }

    public float GetCurrentWaitTime() => waitTime;
    public void AddCurrentWaitTime(float time) => waitTime = time;
}
