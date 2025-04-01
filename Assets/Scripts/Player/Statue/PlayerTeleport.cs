using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerController))]
public class PlayerTeleport : MonoBehaviour
{
    private PlayerController pController;

    [SerializeField] private InputActionReference teleportAction;
    //[SerializeField] private InputActionReference cancelTeleportAction;

    [Header("Teleport Attributes")]
    [Tooltip("Distancia mínima para hacer un tp")]
    [SerializeField] private float teleportRange = 15f;
    
    [Tooltip("Tiempo de canalización del tp")]
    [SerializeField] private float holdTime = 2f;
    
    [Tooltip("Capa que usan las estatuas para ser detectadas por el Raycast")] 
    [SerializeField] private LayerMask statueLayer;

    [Tooltip("Transform de la cámara que se usa para lanzar el Raycast")]
    public Transform CameraTransform;

    // private Coroutine teleportCoroutine;
    // private bool isTeleporting = false;
    private Statue currentStatue;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        HandleStatueDetection(); 
        if (teleportAction.action.WasPerformedThisFrame())
        {
            TryTeleport(); 
        }
    }


    private void OnEnable()
    {
        teleportAction.action.Enable();
    }

    private void OnDisable()
    {
        teleportAction.action.Disable();
    }



    private void HandleStatueDetection()
    {
        Ray ray = new Ray(CameraTransform.position, CameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, teleportRange, statueLayer))
        {
            Statue statue = hit.collider.GetComponent<Statue>();
            if (statue != null)
            {
                if (statue != currentStatue)
                {
                    if (currentStatue != null)
                        currentStatue.ShowUI(false);

                    currentStatue = statue;
                    currentStatue.ShowUI(true);
                }

                return;
            }
        }
        if (currentStatue != null)
        {
            currentStatue.ShowUI(false);
            currentStatue = null;
        }
    }

    private void TryTeleport()
    {
        if (currentStatue != null)
        {
            transform.position = currentStatue.teleportPoint.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (CameraTransform == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(CameraTransform.position, CameraTransform.forward * teleportRange);
    }

}
