using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerBackstabController : MonoBehaviour
{
    [SerializeField] private InputActionReference attackInput;

    [Header("Backstab Attributes")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float backstabDotOffset = 0.65f;
    [SerializeField] private float maxViewAngle = 60;
    [SerializeField] private LayerMask enemyLayer;

    private HashSet<ICanBackstab> enemies = new HashSet<ICanBackstab>();
    private ICanBackstab target;
    private Vector3 sphereOffset = new Vector3(0f, 1f, 0f);

    void Update()
    {
        GatherEnemies();
        CheckIfCanBackstab();
        PerformBackstab();
    }

    private void GatherEnemies()
    {
        Collider[] enemiesAround = Physics.OverlapSphere(transform.position + sphereOffset, attackRange, enemyLayer);
        HashSet<ICanBackstab> newEnemies = new HashSet<ICanBackstab>();

        foreach (Collider collider in enemiesAround)
        {
            if (collider.TryGetComponent(out ICanBackstab enemy))
                newEnemies.Add(enemy);
        }

        foreach (ICanBackstab enemy in enemies)
        {
            if (!newEnemies.Contains(enemy))
                enemy.SetWeakSpot(false);
        }

        enemies = newEnemies;
    }

    private void CheckIfCanBackstab()
    {
        if (enemies.Count == 0)
        {
            target = null;
            return;
        }

        float bestDot = -1f;
        target = null;

        foreach (ICanBackstab enemy in enemies)
        {
            Transform enemyTransform = enemy.GetTransform();
            enemy.SetWeakSpot(false);

            Vector3 dirFromEnemyToPlayer = (transform.position - enemyTransform.position).normalized;
            float dotBackstab = Vector3.Dot(enemyTransform.forward, dirFromEnemyToPlayer);

            if (dotBackstab > -backstabDotOffset) continue;

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

        if (target != null)
            target.SetWeakSpot(true);
    }

    private void PerformBackstab()
    {
        if (target != null && attackInput.action.triggered)
        {
            target.Backstab();
            enemies.Remove(target);
            target.SetWeakSpot(false);
            target = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
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
