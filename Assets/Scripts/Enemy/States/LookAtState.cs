using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtState : State
{
    [Header("LookAt")]
    public float goToSpeed = 3f;
    private Vector3 targetPos;

    [Header("STATES")] 
    public CheckState checkState;

    public override void InitializeState()
    {
        EnemyAudioManager.StopSound();
        if(eController.isChasingPlayer)
            eController.SetAnimation(AnimationType.Run, false);
        else
            eController.SetAnimation(AnimationType.Walk, false);
        
        eController.SetAgentSpeed(goToSpeed);
        targetPos = eController.GoToSoundSource();
        
        if (eController.IsPointInVision(targetPos))
        {
            eController.StopAgent();
            eController.SwitchToNextState(checkState);
        }
    }

    public override State RunCurrentState()
    {
        targetPos = eController.GoToSoundSource();

        if (eController.IsPointInVision(targetPos) || eController.GetEnemyVelocity() == Vector3.zero)
            return checkState;
        return this;
    }
}
