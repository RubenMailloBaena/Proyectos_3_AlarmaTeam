using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReturnState : State
{
    [Header("PATROL")]
    public float returnSpeed = 3f;
    private Vector3 targetPos;

    [Header("STATES")]
    public IdleState idleState;
    public PatrolState patrolState;

    public Material material;
    
    public override void InitializeState()
    {
        eController.ChangeMaterial(material);
        if (!eController.isIdleEnemy)
        {
            eController.SwitchToNextState(patrolState);
            return;
        }
        
        eController.SetAgentSpeed(returnSpeed);
        targetPos = eController.GoToPreviousPosition();
    }

    public override State RunCurrentState()
    {
        if (eController.ArrivedToPosition(targetPos))
        {
            eController.SetEnemyPosBeforeMoving(Vector3.zero);
            return idleState;
        }
        return this;
    }
}
