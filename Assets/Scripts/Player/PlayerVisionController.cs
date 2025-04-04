using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerVisionController : MonoBehaviour
{
    private PlayerController pController;

    [SerializeField] private InputActionReference visionInput;

    [Header("Vision Attributes")]
    [SerializeField] private float range = 10f;
    [SerializeField] private LayerMask enemyLayer;

    private HashSet<IVisible> enemies = new HashSet<IVisible>();
    private Vector3 sphereOffset = new Vector3(0f, 1f, 0f);


    private float input;
    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }
    
    void Update()
    {
        if (visionInput.action.ReadValue<float>() > 0) 
            GatherEnemies();
        else
            ClearEnemies();
    }

    private void GatherEnemies()
    {
        Collider[] enemiesAround = Physics.OverlapSphere(transform.position + sphereOffset, range, enemyLayer);
        HashSet<IVisible> newEnemies = new HashSet<IVisible>();

        foreach (Collider collider in enemiesAround)
        {
            if (collider.TryGetComponent(out IVisible enemy))
            {
                newEnemies.Add(enemy);
                enemy.SetVisiblity(true);
            }
        }

        foreach (IVisible enemy in enemies)
        {
            if (!newEnemies.Contains(enemy))
                enemy.SetVisiblity(false);
        }

        enemies = newEnemies;
        pController.SetUsingVision(true);
    }

    private void ClearEnemies()
    {
        foreach (IVisible enemy in enemies)
            enemy.SetVisiblity(false);
        
        pController.SetUsingVision(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + sphereOffset, range);
    }
}
