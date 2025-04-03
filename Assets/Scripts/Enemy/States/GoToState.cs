using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToState : State
{
    [Header("GoTo")]
    public float goToSpeed = 3f;
    private Vector3 targetPos;

    [Header("STATES")] 
    public CheckState checkState;
    
    public override void InitializeState()
    {
        eController.SetAgentSpeed(goToSpeed);
        targetPos = eController.GoToSoundSource();
    }

    public override State RunCurrentState()
    {
        if (eController.ArrivedToPosition(targetPos))
            return checkState;
        return this;
    }
}
