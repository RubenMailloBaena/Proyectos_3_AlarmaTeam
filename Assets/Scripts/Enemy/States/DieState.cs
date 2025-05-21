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
        Vector3 dirToEnemy = transform.forward;

        Vector3 intendedPosition = transform.position - dirToEnemy * acercarse;
        Quaternion lookRotation = Quaternion.LookRotation(dirToEnemy);

        Vector3 startCheck = transform.position;
        float checkRadius = 0.3f;
        LayerMask wallMask = LayerMask.GetMask("Ground", "Vault"); 

        RaycastHit hit;
        bool wallDetected = Physics.SphereCast(startCheck, checkRadius, -dirToEnemy, out hit, acercarse, wallMask);

        Vector3 finalPosition = intendedPosition;

        if (wallDetected)
        {
            float safeDistance = hit.distance - 0.05f;
            finalPosition = transform.position - dirToEnemy * Mathf.Max(0f, safeDistance);
        }

        float duration = placmentDuration;
        float timer = 0f;

        Vector3 startPos = playerTransform.position;
        Quaternion startRot = playerTransform.rotation;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            playerTransform.position = Vector3.Lerp(startPos, finalPosition, t);
            playerTransform.rotation = Quaternion.Slerp(startRot, lookRotation, t);
            yield return null;
        }

        playerTransform.position = finalPosition;
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
