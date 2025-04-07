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

    [Header("Visual circle")]
    [SerializeField] private GameObject visionCircle;

    private Vector3 sphereOffset = new Vector3(0f, 1f, 0f);

    private float input;
    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        if (visionCircle != null)
        {
            float diameter = range * 2f;
            float scaleFactor = diameter / 10f;
            visionCircle.transform.localScale = new Vector3(scaleFactor, 1f, scaleFactor);
        }
    }

    void Update()
    {
        if (pController.IsPlayerDead)
        {
            showCircle(false);
            return;
        }
        
        if (visionInput.action.ReadValue<float>() > 0)
        {
            GatherEnemies();
            showCircle(true);
        }
        else
        {
            ClearEnemies();
            showCircle(false);
        }
            
    }

    private void showCircle(bool show)
    {
        if (visionCircle != null)
            visionCircle.SetActive(show);
    }

    private void GatherEnemies()
    {
        foreach (IVisible vision in pController.GetVisionObjects())
        {
            if (pController.GetDistance(vision.GetVisionPosition()) <= range)
                vision.SetVisiblity(true);
            else
                vision.SetVisiblity(false);
        }
        
        pController.SetUsingVision(true);
    }

    private void ClearEnemies()
    {
        foreach (IVisible enemy in pController.GetVisionObjects())
            enemy.SetVisiblity(false);
        
        pController.SetUsingVision(false);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + sphereOffset, range);
    }
}
