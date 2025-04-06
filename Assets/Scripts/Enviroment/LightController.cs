using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    
    private PlayerController pController;

    [SerializeField] private float playerOffset = 0.1f;
   
    void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
    }

    void Update()
    {
        Vector3 playerPosition = pController.GetPlayerPosition();
        playerPosition.y += playerOffset;
        Vector3 direction = (playerPosition - transform.position).normalized;

        if(Physics.Raycast(transform.position, direction, out RaycastHit hit))
        {
            Debug.DrawLine(transform.position, hit.point, Color.cyan);

            if (hit.collider.CompareTag("Player"))
                pController.TakeDamage();
        }
    }
}
