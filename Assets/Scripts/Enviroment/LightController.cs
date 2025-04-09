using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private PlayerController pController;
    private bool playerInLight;

    void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
    }

    void Update()
    {
        LightPlayer();
        /*Vector3 playerPosition = pController.GetPlayerPosition();
        playerPosition.y += playerOffset;
        Vector3 direction = (playerPosition - transform.position).normalized;

        if(Physics.Raycast(transform.position, direction, out RaycastHit hit))
        {
            Debug.DrawLine(transform.position, hit.point, Color.cyan);

            print(hit.collider.name);
            if (hit.collider.CompareTag("Player"))
                pController.TakeDamage();
        }*/
    }

    private void LightPlayer()
    {
        if (!playerInLight) return;
        if(PerformRaycast(pController.GetPlayerPosition())) return;
        if (PerformRaycast(pController.GetPlayerBodyPosition())) return;
        PerformRaycast(pController.GetPlayerHeadPosition());
    }

    private bool PerformRaycast(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit))
        {
            Debug.DrawLine(transform.position, hit.point, Color.cyan);
            if (hit.collider.CompareTag("Player"))
            {
                pController.TakeDamage();
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
