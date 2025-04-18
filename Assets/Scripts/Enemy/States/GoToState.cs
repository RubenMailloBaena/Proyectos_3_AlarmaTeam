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
        eController.Renderer.ChangeMaterial(material);
        eController.Movement.SetAgentSpeed(goToSpeed);
        targetPos = eController.Movement.GoToSoundSource();
    }

    public override State RunCurrentState()
    {
        if (eController.Movement.ArrivedToPosition(targetPos))
            return checkState;
        return this;
    }
}
