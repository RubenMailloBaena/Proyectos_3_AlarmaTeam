using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharmState : State
{
    [Header("Charm Attributes")] 
    [SerializeField] private float charmSpeed = 2f;
    [SerializeField] private float rotateSpeed = 3f;
    [SerializeField] private float kickAnimationDuration = 0.5f;
    private Vector3 targetPos, direction;
    private bool setDestination;

    [Header("STATES")] 
    public CheckState checkState;

    private Coroutine performingKick;
    private bool kickDone;
    
    public override void InitializeState()
    {
        EnemyAudioManager.SetSound(SoundType.Charm, transform.position);
        eController.SetAnimation(AnimationType.Charm, true);
        kickDone = false;
        if (performingKick != null)
        {
            StopCoroutine(performingKick);
            performingKick = null;
        }
        
        eController.HideArrow();
        eController.ManualRotation(false);
        eController.StopAgent();
        eController.SetAgentSpeed(charmSpeed);
        setDestination = false;
    }

    public override State RunCurrentState()
    {
        targetPos = eController.GetLeverPosition();
        targetPos.y = transform.position.y;
        direction = (targetPos - transform.position).normalized;
        
        if (eController.RotateEnemy(direction, rotateSpeed))
        {
            if (!setDestination)
            {
                setDestination = true;
                eController.SetAnimation(AnimationType.CharmWalk, false);
                eController.GoToLever();
            }

            if (eController.ArrivedToLever(targetPos))
            {
                eController.StopAgent();
                if(performingKick == null)
                    performingKick = StartCoroutine(PerformKick());                
                if(kickDone)
                    return checkState;
            }
        }
        return this;
    }

    private IEnumerator PerformKick()
    {
        eController.SetAnimation(AnimationType.Charm, false);
        yield return new WaitForSeconds(kickAnimationDuration);
        eController.InteractLever();
        eController.SetCharmLockedVisual(false);
        kickDone = true;
    }
}
