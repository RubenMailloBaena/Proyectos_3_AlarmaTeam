using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DieState : State
{
    [SerializeField] private float placmentDuration = 0.4f;
    [SerializeField] private float waitTimeFromPlacementToKill = 0.5f;
    [SerializeField] private float acercarse = 1.5f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.5f;
    [SerializeField] private ParticleSystem bloodParticles;
    
    private PlayerController player;
    private PlayerHUDController hud;
    private Coroutine killAnimationC;
    
    public override void InitializeState()
    {
        player = GameManager.GetInstance().GetPlayerController();
        hud = GameManager.GetInstance().GetPlayerHUD();
        
        eController.EnemyDead();
        eController.StopAgent();
        //OPTIONAL
        eController.SetWeakSpot(true);

        if(killAnimationC == null)
            killAnimationC = StartCoroutine(KillAnimation());
    }

    public override State RunCurrentState()
    {
        return this;
    }

    private IEnumerator KillAnimation()
    {
        eController.SetAnimation(AnimationType.Idle, true);
        hud.ShowCrossHair(false);

        Transform playerTransform = player.transform;

        // Dirección hacia adelante del enemigo (ya alineado hacia adelante por animación)
        Vector3 dirToEnemy = transform.forward;
        Vector3 posToMovePlayer = transform.position - dirToEnemy * acercarse;
        Quaternion lookRotation = Quaternion.LookRotation(dirToEnemy);

        float duration = placmentDuration;
        float timer = 0f;

        Vector3 startPos = playerTransform.position;
        Quaternion startRot = playerTransform.rotation;

        // Movimiento + rotación simultáneos
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            playerTransform.position = Vector3.Lerp(startPos, posToMovePlayer, t);
            playerTransform.rotation = Quaternion.Slerp(startRot, lookRotation, t);
            yield return null;
        }
        
        playerTransform.position = posToMovePlayer;
        playerTransform.rotation = lookRotation;

        yield return new WaitForSeconds(waitTimeFromPlacementToKill);
        
        player.ShakeCamera(shakeDuration, shakeMagnitude);
        bloodParticles.Play();
        eController.SetWeakSpot(false);
        eController.SetAnimation(AnimationType.Dead, true);


        player.SetBackstabing(false);
        killAnimationC = null;
        hud.ShowCrossHair(true);
    }
}
