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
        eController.ManualRotation(false);
        eController.ChangeMaterial(material);

        if (!eController.SoundWasAnObject())
            eController.SetInPlayerHearState(true);
        else
            eController.SetIgnorePlayerInMinVision(true);
         
        targetPos = eController.SoundPos();
        
        //SI el path esta muy lejos para llegar, ingoramos el sonido
        if(eController.GetPathLength(targetPos) > maxDistanceToIgnore)
            eController.ReturnToLastState();

        eController.StopAgent();
    }

    public override State RunCurrentState()
    {
        targetPos = eController.SoundPos();
        targetDir = (targetPos - transform.position).normalized;
        if (eController.RotateEnemy(targetDir, hearRotationSpeed))
        {
            eController.SetInPlayerHearState(false);
            eController.SetIgnorePlayerInMinVision(false);
            if (eController.SoundWasAnObject())
                return goToState;
            return lookAtState;
        }
        return this;
    }
}
