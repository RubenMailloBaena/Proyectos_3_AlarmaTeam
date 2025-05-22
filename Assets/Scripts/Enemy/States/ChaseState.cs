using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    [Header("ChaseAttributes")]
    [SerializeField] private float knightSpeed = 10f;
    [SerializeField] private float priestSpeed = 6f;
    [SerializeField] private float chaseTimeAfterLoosingVision = 0.5f;
    private float currentTime;
    private Vector3 playerPosition;
    private Vector3 playerPos;

    [Header("STATES")] 
    public AttackState attackState;
    public GoToState goToState;
    public LookAtState lookAtState;

    public override void InitializeState()
    {
        eController.SetAnimation(AnimationType.Run, false);
        
        eController.ActivateExclamation();
        eController.ManualRotation(false);
        eController.SetLight(false);
        switch (eController.enemyType)
        {
            case EnemyType.Knight: eController.SetAgentSpeed(knightSpeed);
                break;
            default: eController.SetAgentSpeed(priestSpeed); 
                break;
        }
        eController.isChasingPlayer = true;
        currentTime = chaseTimeAfterLoosingVision;
        SetSoundPosition();
    }
    

    public override State RunCurrentState()
    {
        if (eController.IsPlayerInVision())
            currentTime = chaseTimeAfterLoosingVision;
        
        if (currentTime <= 0.0f)
        {
            switch (eController.enemyType)
            {
                case EnemyType.Knight: return goToState;
                default: return lookAtState;
            }
        }

        currentTime -= Time.deltaTime;
        SetSoundPosition();
        return this;
    }

    private void SetSoundPosition()
    {
        playerPos = eController.GoToPlayerPosition();
        eController.SetSoundPos(playerPos);
    }
}
