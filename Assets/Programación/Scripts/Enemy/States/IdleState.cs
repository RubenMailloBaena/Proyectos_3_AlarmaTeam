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

    private bool doneAnimation;

    private void Start()
    {
        if(!eController.isIdleEnemy) eController.SwitchToNextState(patrolState);
    }

    public override void InitializeState()
    {
        eController.StopAllSounds();
        eController.PlaySound(SoundType.Idle);
        eController.SetAnimation(AnimationType.Idle, false);
        
        waitTime = eController.GetWaitTime();
        lookDir = eController.GetLookDirection();
        doneAnimation = false;

        if (waitTime == 0) return;
        
        if (lookDir != Vector3.zero)
        {
            Vector3 cross = Vector3.Cross(transform.forward, lookDir.normalized);
            if (cross.y > 0)
                eController.SetAnimation(AnimationType.TurnRight, false);
            else if (cross.y < 0)
                eController.SetAnimation(AnimationType.TurnLeft, false);
            else
                eController.SetAnimation(AnimationType.Idle, false);
        }
    }
    
    public override State RunCurrentState()
    {
        if(lookDir != Vector3.zero)
            if (eController.RotateEnemy(lookDir, idleRotationSpeed))
                eController.SetAnimation(AnimationType.Idle, false);
        
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
