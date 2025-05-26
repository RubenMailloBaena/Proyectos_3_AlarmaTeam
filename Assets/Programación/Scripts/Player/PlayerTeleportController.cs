using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.VFX;

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
    
    [Header("Animation References & Attributes")]
    [SerializeField] private VisualEffect smokeEffect;
    [SerializeField] private float minFOV = 50;
    [SerializeField] private float waitOnMax = 0.2f;
    private Camera playerCamera;
    private float smokeDefaultClip, defaultFov;
    private bool isDissipating = false;
    
    [Tooltip("Transform de la c�mara que se usa para lanzar el Raycast")]
    public Transform leanParent;

    private Coroutine teleportCoroutine;
    private Coroutine smokeResetCoroutine;
    private IStatue currentStatue;
    private float input;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
        smokeDefaultClip = smokeEffect.GetFloat("Clip");
        playerCamera = Camera.main;
        defaultFov = playerCamera.fieldOfView;
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
        
        AudioManager.Instance.HandleStopSound("event:/Jugador/jugador_habilidad_paso_sombrio", true);

        if (teleportCoroutine != null)
        {
            StopCoroutine(teleportCoroutine);
            teleportCoroutine = null;
            pController.HideProgressBar();
        }

        if (!isDissipating)
        {
            if (smokeResetCoroutine != null) StopCoroutine(smokeResetCoroutine);
            smokeResetCoroutine = StartCoroutine(LerpClipToDefault(false));
        }

        pController.SetTeleporting(false);
    }

    private IEnumerator TeleportAfterHold()
    {
        print("TELEPORTING");
        if (smokeResetCoroutine != null) StopCoroutine(smokeResetCoroutine);
        isDissipating = false;
        AudioManager.Instance.HandlePlaySound3D("event:/Jugador/jugador_habilidad_paso_sombrio", transform.position);
        pController.SetTeleporting(true);

        float elapsed = 0f;
        float startClip = smokeEffect.GetFloat("Clip");
        float endClip = 0f;

        float initialFOV = playerCamera.fieldOfView;

        while (elapsed < holdTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / holdTime);

            smokeEffect.SetFloat("Clip", Mathf.Lerp(startClip, endClip, t));
            playerCamera.fieldOfView = Mathf.Lerp(initialFOV, minFOV, t);

            print(playerCamera.fieldOfView);
            
            pController.UpdateProgressBar(elapsed);
            yield return null;
        }

        if (currentStatue != null)
        {
            transform.position = currentStatue.GetTPPoint();
            transform.rotation = currentStatue.GetTPRotation();
        }

        pController.PlayerTP();
        AudioManager.Instance.HandleStopSound("event:/Jugador/jugador_habilidad_paso_sombrio", true);
        pController.SetTeleporting(false);

        //if (smokeResetCoroutine != null) StopCoroutine(smokeResetCoroutine);
        smokeResetCoroutine = StartCoroutine(LerpClipToDefault(true));
    }
    
    
    private IEnumerator LerpClipToDefault(bool wait)
    {
        print("CLEARING");
        isDissipating = true;
        float currentClip = smokeEffect.GetFloat("Clip");
        float initialFOV = playerCamera.fieldOfView;
        
        if(wait)
            yield return new WaitForSeconds(waitOnMax); 

        float elapsed = 0f;
        float duration = 0.5f; 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            smokeEffect.SetFloat("Clip", Mathf.Lerp(currentClip, smokeDefaultClip, t));
            playerCamera.fieldOfView = Mathf.Lerp(initialFOV, defaultFov, t); 
            print(playerCamera.fieldOfView);
            yield return null;
        }

        smokeEffect.SetFloat("Clip", smokeDefaultClip);
        playerCamera.fieldOfView = defaultFov; 
        isDissipating = false; 
    }

    private void OnDrawGizmosSelected()
    {
        if (leanParent == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(leanParent.position, leanParent.forward * teleportRange);
    }
}
