using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerController))]
public class PlayerTeleportController : MonoBehaviour, IPlayerComponent
{
    private PlayerController pController;

    [SerializeField] private InputActionReference teleportAction;

    [Header("Teleport Attributes")]
    [Tooltip("Distancia m�nima para hacer un tp")]
    [SerializeField] private float teleportRange = 15f;
    
    [Tooltip("Tiempo de canalizaci�n del tp")]
    [SerializeField] private float holdTime = 2f;
    [SerializeField] private LayerMask allLayers;

    
    [Tooltip("Transform de la c�mara que se usa para lanzar el Raycast")]
    public Transform leanParent;

    private Coroutine teleportCoroutine;
    private IStatue currentStatue;
    private float input;

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
        if (pController.CanVault || pController.IsVaulting || pController.isBackstabing)
        {
            ClearTarget();
            return;
        }

        Ray ray = new Ray(leanParent.position, leanParent.forward);
       
        if (Physics.Raycast(ray, out RaycastHit hit, teleportRange, allLayers))
        {
            if (hit.transform.TryGetComponent(out IStatue statue))
            {
                currentStatue = statue;
                currentStatue.ShowUI(true);
                pController.CanInteract(teleportAction, InputType.Hold, this, ActionType.Teleport);
            }
            else
               ClearTarget();
        }
        else
            ClearTarget();
    }

    void ClearTarget()
    {
        if (currentStatue != null)
        {
            currentStatue.ShowUI(false);
            currentStatue = null;
            pController.HideInteract(this);
        }
    }

    private void PerformTeleport()
    {
        if (currentStatue == null)
        {
            CancelTeleport();
            return;
        }
       
        input = teleportAction.action.ReadValue<float>();
            
        
        if (input>0 && !pController.IsTeleporting && !pController.IsGamePaused)
            teleportCoroutine = StartCoroutine(TeleportAfterHold());
        
        else if(input == 0 && pController.IsTeleporting)
            CancelTeleport();
    }

    private void CancelTeleport()
    {
        AudioManager.Instance.HandleStopSound("event:/Jugador/jugador_habilidad_paso_sombrio", false);
        if (teleportCoroutine != null)
        {
            StopCoroutine(teleportCoroutine);
            pController.HideProgressBar();
        }
        pController.SetTeleporting(false);
    }

    private IEnumerator TeleportAfterHold()
    {
        AudioManager.Instance.HandlePlaySound3D("event:/Jugador/jugador_habilidad_paso_sombrio", transform.position);
        pController.SetTeleporting(true);
        float elapsed = 0;

        while (elapsed < holdTime)
        {
            elapsed += Time.deltaTime;
            pController.UpdateProgressBar(elapsed);
            yield return null;
        }

        if (currentStatue != null)
        {
            transform.position = currentStatue.GetTPPoint();
            transform.rotation = currentStatue.GetTPRotation();
        }
            
        pController.PlayerTP();
        AudioManager.Instance.HandleStopSound("event:/Jugador/jugador_habilidad_paso_sombrio", false);
        pController.SetTeleporting(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (leanParent == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(leanParent.position, leanParent.forward * teleportRange);
    }
}
