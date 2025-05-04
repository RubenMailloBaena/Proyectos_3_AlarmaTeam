using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private PlayerController pController;
    private bool playerInLight;
    public LayerMask allLayer;
    public float damage;
    
    private Vector3 offset = new Vector3(0, 0.20f, 0);

    void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
    }

    void Update()
    {
        LightPlayer();
    }

    private void LightPlayer()
    {
        if (!playerInLight) return;
        if (PerformRaycast(pController.GetPlayerPosition() + offset)) return;
        if (PerformRaycast(pController.GetPlayerBodyPosition())) return;
        PerformRaycast(pController.GetPlayerHeadPosition());
    }

    private bool PerformRaycast(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, allLayer))
        {
            print("HITTING PLAYER: " + hit.collider.name);
            Debug.DrawLine(transform.position, hit.point, Color.cyan);
            if (hit.collider.CompareTag("Player"))
            {
                
                pController.TakeDamage(damage);
                return true;
            }
        }
        return false;
    }

private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            playerInLight = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            playerInLight = false;
    }
}
