using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearState : State
{
    private Vector3 targetPos, targetDir;
    
    [Header("HEAR ATTRIBUTES")]
    [SerializeField] private float hearRotationSpeed = 5f;
    [SerializeField] private float maxDistanceToIgnore = 10f;
    
    [Header("STATES")]
    public GoToState goToState;
    public LookAtState lookAtState;

    public override void InitializeState()
    {
        eController.StopSound();
        eController.HideArrow();
        eController.ManualRotation(false);

        if (!eController.SoundWasAnObject())
            eController.SetInPlayerHearState(true);
        else
            eController.SetIgnorePlayerInMinVision(true);
         
        targetPos = eController.SoundPos();
        
        //SI el path esta muy lejos para llegar, ingoramos el sonido
        if(eController.GetPathLength(targetPos) > maxDistanceToIgnore)
            eController.ReturnToLastState();
        
        targetDir = (targetPos - transform.position).normalized;
        Vector3 cross = Vector3.Cross(transform.forward, targetDir);
        if (cross.y > 0)
            eController.SetAnimation(AnimationType.TurnRight, false);
        else if(cross.y < 0)
            eController.SetAnimation(AnimationType.TurnLeft, false);
        else
            eController.SetAnimation(AnimationType.Idle, false);
        
        eController.StopAgent();
    }

    public override State RunCurrentState()
    {
        targetPos = eController.SoundPos();
        targetDir = (targetPos - transform.position).normalized;
        if (eController.RotateEnemy(targetDir, hearRotationSpeed))
        {
            eController.SetAnimation(AnimationType.StopTurn, false);
            eController.SetInPlayerHearState(false);
            eController.SetIgnorePlayerInMinVision(false);
            if (eController.SoundWasAnObject())
                return goToState;
            return lookAtState;
        }
        return this;
    }
}
