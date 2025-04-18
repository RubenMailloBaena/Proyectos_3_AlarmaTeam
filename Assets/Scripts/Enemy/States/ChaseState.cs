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

    public Material material;
    
    public override void InitializeState()
    {
        eController.ActivateExclamation();
        eController.Renderer.ChangeMaterial(material);
        eController.Movement.ManualRotation(false);
        eController.Renderer.SetLight(false);
        switch (eController.enemyType)
        {
            case EnemyType.Knight: eController.Movement.SetAgentSpeed(knightSpeed);
                break;
            default: eController.Movement.SetAgentSpeed(priestSpeed); 
                break;
        }
        eController.isChasingPlayer = true;
        currentTime = chaseTimeAfterLoosingVision;
        SetSoundPosition();
    }
    

    public override State RunCurrentState()
    {
        if (eController.Vision.isPlayerInVision)
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
        playerPos = eController.Movement.GoToPlayerPosition();
        eController.Hear.soundPos = playerPos;
    }
}
