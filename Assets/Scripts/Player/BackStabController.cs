using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class BackStabController : MonoBehaviour
{
    [SerializeField] private InputActionReference attackInput;

    [Header("Backstab Attributes")]
    [Tooltip("Distancia minima para hacer un backStab")]
    [SerializeField] private float attackRange = 2f;

    [Tooltip("Cuanto destras del enemigo tenemos que estar para poder hacer backstab (cuanto más bajo más ángulo)")]
    [SerializeField] private float backstabDotOffset = 0.65f;

    [Tooltip("Cono de Vision del player en el que puede hacer el backstab (60º para quitarlo)")]
    [SerializeField] private float maxViewAngle = 60;

    private SphereCollider backstabCollider;
    private HashSet<ICanBackstab> enemies = new HashSet<ICanBackstab>();
    private ICanBackstab target;

    void Awake()
    {
        backstabCollider = GetComponent<SphereCollider>();
        backstabCollider.radius = attackRange; 
    }

    void Update()
    {
        CheckIfCanBackstab();
        PerformBackstab();
    }

    private void CheckIfCanBackstab()
    {
        if (enemies.Count == 0) return;

        float bestDot = -1f;
        target = null;

        foreach (ICanBackstab enemy in enemies)
        {
            Transform enemyTransform = enemy.GetTransform();
            enemy.SetWeakSpot(false); 

            // COMPROBAMOS QUE ESTAMOS DETRAS DEL ENEMIGO
            Vector3 dirFromEnemyToPlayer = (transform.position - enemyTransform.position).normalized;
            float dotBackstab = Vector3.Dot(enemyTransform.forward, dirFromEnemyToPlayer);

            if (dotBackstab > -backstabDotOffset) continue;

            // ESTA DENTRO DE NUESTRO RANGO DE VISION
            Vector3 dirToEnemy = (enemyTransform.position - transform.position).normalized;
            float dotView = Vector3.Dot(transform.forward, dirToEnemy);

            float minDotView = Mathf.Cos(maxViewAngle * Mathf.Deg2Rad);
            if (dotView < minDotView) continue; 

            // SI HAY MAS DE UN ENEMIGO, PONEMOS COMO TARGET AL ENEMIGO QUE ESTE MAS CENTRADO EN NUESTRA CAMARA
            if (dotView > bestDot)
            {
                bestDot = dotView;
                target = enemy;
            }
        }

        if (target != null)
        {
            target.SetWeakSpot(true);
        }
    }

    private void PerformBackstab()
    {
        if (target != null && attackInput.action.triggered)
        {
            target.Backstab();
            ClearTarget(target);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && other.TryGetComponent(out ICanBackstab enemy))
        {
            enemies.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && other.TryGetComponent(out ICanBackstab enemy))
        {
            ClearTarget(enemy);
        }
    }

    private void ClearTarget(ICanBackstab enemy)
    {
        if (enemy == target)
        {
            target = null;
            enemy.SetWeakSpot(false);
        }
        enemies.Remove(enemy);
    }

    // INPUTS
    private void OnEnable()
    {
        attackInput.action.Enable();
    }

    private void OnDisable()
    {
        attackInput.action.Disable();
    }
}