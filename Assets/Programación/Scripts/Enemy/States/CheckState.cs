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

    private bool lookingLeft, finishedWaiting, returningToStart, animationDone;
    private Vector3 startingForward, rightLookDir, leftLookDir;

    public override void InitializeState()
    {
        print("CHECK STATE");
        
        eController.SetAnimation(AnimationType.Idle, false);
        eController.StopSound();
        eController.SetSoundWasAnObject(true);//RESTART PLAYER HEAR
        eController.isChasingPlayer = false;
        eController.exclamationShown = false;
        eController.SetLight(false);
        eController.ManualRotation(false);
        eController.StopAgent();

        startingForward = transform.forward;
        rightLookDir = Quaternion.Euler(0, checkAngle, 0) * startingForward;
        leftLookDir = Quaternion.Euler(0, -checkAngle, 0) * startingForward;
        
        finishedWaiting = false;
        returningToStart = false;
        animationDone = false;

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
                if (!animationDone)
                {
                    animationDone = true;
                    eController.SetAnimation(AnimationType.TurnRight, false);
                }

                if (eController.RotateEnemy(rightLookDir, checkRotateSpeed))
                {
                    lookingLeft = true;
                    animationDone = false;
                }
            }
            else //MIRAMOS A LA IZQUIERDA
            {
                if (!animationDone)
                {
                    animationDone = true;
                    eController.SetAnimation(AnimationType.TurnLeft, false);
                }
                if (eController.RotateEnemy(leftLookDir, checkRotateSpeed))
                {
                    lookingLeft = false;
                    returningToStart = true;
                    animationDone = false;
                }
            }
        }
        //QUIZAS QUITARLO
        else //CUANDO HEMOS MIRADO A LOS DOS LADOS, VOLVEMOS A MIRAR ADELANTE ANTES DE CAMBIAR DE ESTADO
        {
            if (!animationDone)
            {
                animationDone = true;
                eController.SetAnimation(AnimationType.TurnRight, false);
            }
            
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
