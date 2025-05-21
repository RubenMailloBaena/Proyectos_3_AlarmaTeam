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

    public Material material;
    
    public override void InitializeState()
    {
        eController.SetAnimation(AnimationType.Walk, false);
        
        eController.ChangeMaterial(material);
        eController.SetAgentSpeed(goToSpeed);
    }

    public override State RunCurrentState()
    {
        targetPos = eController.GoToSoundSource();
        if (eController.ArrivedToPosition(targetPos))
            return checkState;
        return this;
    }
}
