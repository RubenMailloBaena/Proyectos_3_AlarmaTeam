using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerVisionController : MonoBehaviour
{
    private PlayerController pController;

    [SerializeField] private InputActionReference visionInput;

    [Header("Vision Attributes")]
    [SerializeField] private float range = 10f;

    private HashSet<IVisible> enemies = new HashSet<IVisible>();
    private Vector3 sphereOffset = new Vector3(0f, 1f, 0f);

    private float input;
    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }
    
    void Update()
    {
        if (pController.IsPlayerDead) return;
        
        if (visionInput.action.ReadValue<float>() > 0) 
            GatherEnemies();
        else
            ClearEnemies();
    }

    private void GatherEnemies()
    {
        foreach (IVisible vision in enemies)
        {
            float distance = Vector3.Distance(transform.position, vision.GetPosition());
            if (distance <= range)
                vision.SetVisiblity(true);
            else
                vision.SetVisiblity(false);
        }
        
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

    private void AddVision(IVisible visible)
    {
        enemies.Add(visible);
    }

    private void OnEnable()
    {
        pController.onVisionSubsrcribe += AddVision;
    }

    private void OnDisable()
    {
        pController.onVisionSubsrcribe -= AddVision;
    }
}
