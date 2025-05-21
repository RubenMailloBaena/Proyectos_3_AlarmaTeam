using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : State
{
    [Header("Attack Attributes")]
    [SerializeField] private float priestSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 5f;

    public float damage;

    [Header("STATES")] 
    public ChaseState chaseState;

    private Vector3 playerPos;
    private PlayerController pController;

    public Material material;
    
    public override void InitializeState()
    {
        eController.SetAnimation(AnimationType.Attack, true);
        
        eController.ActivateExclamation();
        eController.ChangeMaterial(material);
        eController.ManualRotation(true);
        eController.isChasingPlayer = false;
        pController = eController.GetPlayerController();

        eController.SetAgentSpeed(priestSpeed);
        playerPos = eController.GoToPlayerPosition();
    }

    public override State RunCurrentState()
    {
        playerPos = eController.GoToPlayerPosition();
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos);

        Vector3 dir = (playerPos - transform.position).normalized;
        eController.RotateEnemy(dir, rotationSpeed);
        
        if (eController.IsPlayerInVision() && distanceToPlayer <= eController.GetExitAttackRange())
        {
            eController.SetLight(true);
            pController.TakeDamage(damage);
            if (pController.IsPlayerDead)
            {
                eController.KillPlayer();
            }
            return this;
        }
        return chaseState;
    }
}
