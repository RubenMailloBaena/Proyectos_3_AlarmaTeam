using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerBackstabController : MonoBehaviour, IPlayerComponent
{
    private PlayerController pController;

    [SerializeField] private InputActionReference attackInput;

    [Header("Backstab Attributes")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float backstabDotOffset = 0.65f;
    [SerializeField] private float maxViewAngle = 60;
    [SerializeField] private LayerMask enemyLayer;

    private IEnemyBackstab target;
    private Vector3 sphereOffset = new Vector3(0f, 1f, 0f);
    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (pController.IsUsingVision) return;
        CheckIfCanBackstab();
        PerformBackstab();
    }

    private void CheckIfCanBackstab()
    {
        float bestDot = -1f;
        target = null;
        
        foreach (IEnemyBackstab enemy in pController.GetBackstabEnemies())
        {
            enemy.SetWeakSpot(false);
            if (pController.GetDistance(enemy.GetTransform().position) > attackRange) continue;
            
            Transform enemyTransform = enemy.GetTransform();

            //COMPROBAMOS SI ESTAMOS DETRAS
            Vector3 dirFromEnemyToPlayer = (transform.position - enemyTransform.position).normalized;
            float dotBackstab = Vector3.Dot(enemyTransform.forward, dirFromEnemyToPlayer);

            if (dotBackstab > -backstabDotOffset) continue;

            //SI ESTA EN NUESTRO CONO DE VISION, Y SI HAY MAS DE UN ENEMIGO, ELEGIMOS AL QUE ESTAMOS MIRANDO
            Vector3 dirToEnemy = (enemyTransform.position - transform.position).normalized;
            float dotView = Vector3.Dot(transform.forward, dirToEnemy);

            float minDotView = Mathf.Cos(maxViewAngle * Mathf.Deg2Rad);
            if (dotView < minDotView) continue;

            if (dotView > bestDot)
            {
                bestDot = dotView;
                target = enemy;
            }
        }

        if (target != null && !target.IsEnemyDead())
        {
            target.SetWeakSpot(true);
            pController.CanInteract(attackInput.action, InputType.Press, this, ActionType.Backstab);
        }
        else
            pController.HideInteract(this);
    }

    private void PerformBackstab()
    {
        if (target != null && attackInput.action.triggered && !pController.IsGamePaused)
        {
            target.Backstab();
            target.SetWeakSpot(false);
            target = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + sphereOffset, attackRange);
    }

    private void OnEnable()
    {
        attackInput.action.Enable();
    }

    private void OnDisable()
    {
        attackInput.action.Disable();
    }
}
