using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHear : MonoBehaviour, IEnemyHear
{
    private EnemyController eController;

    [HideInInspector] public bool soundWasAnObject = true;
    [HideInInspector] public Vector3 soundPos;
    [HideInInspector] public bool inPlayerHearState = true;
    
    public void SetHear(EnemyController enemyController)
    {
        eController = enemyController;
        inPlayerHearState = true;
        GameManager.GetInstance().GetPlayerController().AddHearEnemy(this);
    }
    
    public void HeardSound(Vector3 soundPoint, bool isObject)
    {
        if (!isObject && Mathf.Abs(soundPoint.y - transform.position.y) > 0.3f 
            || eController.isChasingPlayer || eController.IsAttacking() || eController.IsCharmed()) 
            return;

        eController.Movement.SetPositionBeforeMoving();
        
        soundPos = soundPoint;
        soundPos.y = transform.position.y;

        if (!soundWasAnObject && !isObject && !inPlayerHearState) 
        {
            eController.SwitchToLookAtState();
            return;
        }

        soundWasAnObject = isObject;
        eController.SwitchToHearState();
    }

    private void OnDestroy()
    {
        GameManager.GetInstance().GetPlayerController().RemoveHearEnemy(this);
    }

    public Vector3 GetPosition() => transform.position;
    
    
}
