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

    public Material material;

    private void Start()
    {
        if(!eController.isIdleEnemy) eController.SwitchToNextState(patrolState);
    }

    public override void InitializeState()
    {
        waitTime = eController.Movement.GetWaitTime();
        lookDir = eController.Movement.GetLookDirection();
        eController.Renderer.ChangeMaterial(material);
    }
    
    public override State RunCurrentState()
    {
        if(lookDir != Vector3.zero) 
            eController.Movement.RotateEnemy(lookDir, idleRotationSpeed);
        
        if(eController.isIdleEnemy) return this;
        
        waitTime -= Time.deltaTime;

        if (waitTime <= 0.0f)
        {
            eController.Movement.IncrementIndex();
            return patrolState;
        }
        
        return this;
    }
}
