using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHear : MonoBehaviour, IEnemyHear
{
    private EnemyController eController;

    private bool soundWasAnObject = true;
    private Vector3 soundPos;
    private bool inPlayerHearState = true;
    
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

        eController.SetPositionBeforeMoving();
        
        soundPos = soundPoint;
        soundPos.y = transform.position.y;

        if (ShouldIgnoreSound(isObject)) 
        {
            eController.SwitchToLookAtState();
            return;
        }

        soundWasAnObject = isObject;
        eController.SwitchToHearState();
    }

    private bool ShouldIgnoreSound(bool isObject)
    {
        return !soundWasAnObject && !isObject && !inPlayerHearState;
    }

    private void OnDestroy()
    {
        GameManager.GetInstance().GetPlayerController().RemoveHearEnemy(this);
    }

    public Vector3 GetPosition() => transform.position;
    
    public bool SoundWasAnObject
    {
        get => soundWasAnObject;
        set => soundWasAnObject = value;
    }

    public Vector3 SoundPos
    {
        get => soundPos;
        set => soundPos = value;
    }

    public bool InPlayerHearState
    {
        set => inPlayerHearState = value;
    }
    
}
