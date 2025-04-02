using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToState : State
{
    [Header("GoTo")]
    public float goToSpeed = 3f;
    private Vector3 targetPos;
    
    public override void InitializeState()
    {
        eController.SetAgentSpeed(goToSpeed);
        //targetPos
        //ACABAR LOGICA
    }

    public override State RunCurrentState()
    {
        
    }
}
