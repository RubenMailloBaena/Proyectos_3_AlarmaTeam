using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class CheckState : State
{
    [Header("CHECK ATTRIBUTES")]
    [SerializeField] private float startingIdleTime = 2f;
    [SerializeField] private float checkRotateSpeed = 5f;
    [SerializeField] private float checkAngle = 20f;

    [Header("STATES")] 
    public ReturnState returnState;

    private bool lookingLeft, finishedWaiting, returningToStart;
    private Vector3 startingForward, rightLookDir, leftLookDir;

    public Material material;
    
    public override void InitializeState()
    {
        eController.renderer.material = material;
        eController.soundWasAnObject = true; //RESTART PLAYER HEAR
        eController.isChasingPlayer = false;
        eController.StopAgent();

        startingForward = transform.forward;
        rightLookDir = Quaternion.Euler(0, checkAngle, 0) * startingForward;
        leftLookDir = Quaternion.Euler(0, -checkAngle, 0) * startingForward;
        
        finishedWaiting = false;
        returningToStart = false;

        StartCoroutine(StartingWaitTime());
    }

    public override State RunCurrentState()
    {
        Debug.DrawRay(transform.position, rightLookDir * 5, Color.red);
        Debug.DrawRay(transform.position, leftLookDir * 5, Color.red);

        if (!finishedWaiting) return this;

        if (!returningToStart)
        {
            if (!lookingLeft) //MIRAMOS A LA DERECHA 
            {
                if (eController.RotateEnemy(rightLookDir, checkRotateSpeed))
                    lookingLeft = true;
            }
            else //MIRAMOS A LA IZQUIERDA
            {
                if (eController.RotateEnemy(leftLookDir, checkRotateSpeed))
                {
                    lookingLeft = false;
                    returningToStart = true; 
                }
            }
        }
        //QUIZAS QUITARLO
        else //CUANDO HEMOS MIRADO A LOS DOS LADOS, VOLVEMOS A MIRAR ADELANTE ANTES DE CAMBIAR DE ESTADO
        {
            if (eController.RotateEnemy(startingForward, checkRotateSpeed))
                return returnState;
        }
        return this;
    }

    private IEnumerator StartingWaitTime()
    {
        yield return new WaitForSeconds(startingIdleTime);
        finishedWaiting = true;
    }
}
