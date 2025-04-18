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

    public Material material;

    public override void InitializeState()
    {
        eController.HideArrow();
        eController.Renderer.ChangeMaterial(material);

        if (!eController.soundWasAnObject)
            eController.inPlayerHearState = true;
        else
            eController.Vision.ignorePlayerInMinVision = true;
         
        targetPos = eController.soundPos;
        
        //SI el path esta muy lejos para llegar, ingoramos el sonido
        if(eController.Movement.GetPathLength(targetPos) > maxDistanceToIgnore)
            eController.ReturnToLastState();

        eController.Movement.StopAgent();
        targetDir = (targetPos - transform.position).normalized;
    }

    public override State RunCurrentState()
    {
        if (eController.Movement.RotateEnemy(targetDir, hearRotationSpeed))
        {
            eController.inPlayerHearState = false;
            eController.Vision.ignorePlayerInMinVision = false;
            if (eController.soundWasAnObject)
                return goToState;
            return lookAtState;
        }
        return this;
    }
}
