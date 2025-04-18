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

    public Material material;
    
    public override void InitializeState()
    {
        eController.exclamationShown = false;
        eController.Renderer.ChangeMaterial(material);
        if(!eController.isChasingPlayer)
            eController.Movement.SetAgentSpeed(goToSpeed);
        targetPos = eController.Movement.GoToSoundSource();

        if (eController.IsPointInVision(targetPos))
        {
            eController.Movement.StopAgent();
            eController.SwitchToNextState(checkState);
        }
    }

    public override State RunCurrentState()
    {
        if (eController.IsPointInVision(targetPos) || eController.Movement.GetEnemyVelocity() == Vector3.zero)
            return checkState;
        return this;
    }
}
