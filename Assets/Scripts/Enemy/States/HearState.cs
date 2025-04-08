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
        eController.renderer.material = material;

        if (!eController.soundWasAnObject)
            eController.inPlayerHearState = true;
        else
            eController.ignorePlayerInMinVision = true;
         
        targetPos = eController.soundPos;
        
        //SI el path esta muy lejos para llegar, ingoramos el sonido
        if(eController.GetPathLegth(targetPos) > maxDistanceToIgnore)
            eController.ReturnToLastState();

        eController.StopAgent();
        targetDir = (targetPos - transform.position).normalized;
    }

    public override State RunCurrentState()
    {
        if (eController.RotateEnemy(targetDir, hearRotationSpeed))
        {
            eController.inPlayerHearState = false;
            eController.ignorePlayerInMinVision = false;
            if (eController.soundWasAnObject)
                return goToState;
            return lookAtState;
        }
        return this;
    }
}
