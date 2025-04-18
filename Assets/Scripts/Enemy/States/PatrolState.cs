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

    public Material material;

    public override void InitializeState()
    {
        eController.Movement.SetAgentSpeed(patrolSpeed);
        targetPos = eController.Movement.GoToWaypoint();
        eController.Renderer.ChangeMaterial(material);
    }

    public override State RunCurrentState()
    {
        if (eController.Movement.ArrivedToPosition(targetPos))
            return idleState;
        return this;
    }
}
