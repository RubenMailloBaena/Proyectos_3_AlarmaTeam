using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerController))]
public class PlayerTeleportController : MonoBehaviour
{
    private PlayerController pController;

    [SerializeField] private InputActionReference teleportAction;
    //[SerializeField] private InputActionReference cancelTeleportAction;

    [Header("Teleport Attributes")]
    [Tooltip("Distancia m�nima para hacer un tp")]
    [SerializeField] private float teleportRange = 15f;
    
    [Tooltip("Tiempo de canalizaci�n del tp")]
    [SerializeField] private float holdTime = 2f;
    
    [Tooltip("Capa que usan las estatuas para ser detectadas por el Raycast")] 
    [SerializeField] private LayerMask statueLayer;

    [Tooltip("Transform de la c�mara que se usa para lanzar el Raycast")]
    public Transform leanParent;

    // private Coroutine teleportCoroutine;
    // private bool isTeleporting = false;
    private IStatue currentStatue;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        HandleStatueDetection(); 
        PerformTeleport();
    }

    private void HandleStatueDetection()
    {
        Ray ray = new Ray(leanParent.position, leanParent.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, teleportRange, statueLayer))
        {
            print("HERE");
            if (hit.transform.TryGetComponent(out IStatue statue))
            {
                print("HERE2");
                currentStatue = statue;
                currentStatue.ShowUI(true);
            }
        }
        else
        {
            if (currentStatue != null)
            {
                currentStatue.ShowUI(false);
                currentStatue = null;
            }
        }
    }

    private void PerformTeleport()
    {
        if (currentStatue != null && teleportAction.action.WasPerformedThisFrame())
            transform.position = currentStatue.GetTPPoint();
    }

    private void OnDrawGizmos()
    {
        if (leanParent == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(leanParent.position, leanParent.forward * teleportRange);
    }

    private void OnEnable()
    {
        teleportAction.action.Enable();
    }

    private void OnDisable()
    {
        teleportAction.action.Disable();
    }
}
