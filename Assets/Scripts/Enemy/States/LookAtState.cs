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
        if (eController.IsPointInVision(targetPos))
            return checkState;
        return this;
    }
}
