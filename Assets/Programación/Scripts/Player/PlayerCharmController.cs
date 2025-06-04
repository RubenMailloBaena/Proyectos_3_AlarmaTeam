using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerCharmController : MonoBehaviour, IPlayerComponent
{
    private PlayerController pController;

    [SerializeField] private InputActionReference charmInput;
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private Transform leanParent;
    
    [Header("Charm Attributes")]
    [SerializeField] private float charmRange = 25f;
    
    [Header("Vision Attributes")]
    [SerializeField] private float visionRange = 15f;

    [Header("Visual circle")] 
    [SerializeField] private LayerMask allLayers;
    [SerializeField] private GameObject visionCircle;

    private ICharmEnemy hoveredTarget;
    private ICharmEnemy charmedTarget;
    private List<IInteractable> interactables = new List<IInteractable>();
    private List<ICharmEnemy> charmedEnemies = new List<ICharmEnemy>();
    private List<IInteractable> charmedInteractables = new List<IInteractable>();
    private List<ICharmEnemy> checkpointEnemeis = new List<ICharmEnemy>();
    private List<IInteractable> checkpointInteractables = new List<IInteractable>();
    private bool isCharming;
    private bool isExitingCharm = false;

    private void Awake()
    {
        pController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        if (visionCircle != null)
        {
            float scaleFactor = (charmRange * 2f) / 10f;
            visionCircle.transform.localScale = new Vector3(scaleFactor, 1f, scaleFactor);
        }
    }

    private void Update()
    {
        HandleInput();
        HandleCharmingMode();
        UpdateHoveredTarget();
        UpdateVisionTargets();
        UpdateCharmedTarget();
        HandleCharmedInteraction();
        HandleCharmedObjects();
    }

    #region Input Handling

    private bool IsCharmPressed() => charmInput.action.triggered;
    private bool IsAttackPressed() => attackInput.action.triggered;

    private void HandleInput() //INPUT DEL PLAYER
    {
        if (pController.isBackstabing)
        {
            ExitCharmingMode();
            return;
        }
        
        if (IsCharmPressed() && !pController.IsGamePaused)
            SwapMode();

        //SI HAY UN HOVERED Y APRETAMOS EL INPUT; LO ASIGNAMOS AL CHARMED TARGET
        if (IsAttackPressed() && charmedTarget == null && hoveredTarget != null && !pController.IsGamePaused) 
        {
            charmedTarget = hoveredTarget;
            hoveredTarget = null;
        }
    }

    private void SwapMode()//SALIMOS O ENTRAMOS EN EL MODO DE CHARM
    {
        if (isCharming)
            ExitCharmingMode();
        else
            EnterCharmingMode();
    }

    private void EnterCharmingMode()
    {
        isCharming = true;
        pController.SetCharmingVisual(true);
        visionCircle.SetActive(true);
        AudioManager.Instance.HandlePlay3DOneShot("playerCharm", "event:/Jugador/jugador_vision_vampirica_activar", transform.position);
    }

    private void ExitCharmingMode()
    {
        if (!isCharming || isExitingCharm) return;

        isExitingCharm = true;
        isCharming = false;

        pController.SetCharmingVisual(false);
        visionCircle.SetActive(false);

        ClearHoveredTarget();
        ClearCharmedTarget();
        ClearVisionTargets();

        AudioManager.Instance.HandlePlay3DOneShot("playerCharm", "event:/Jugador/jugador_vision_vampirica_desactivar", transform.position);

        StartCoroutine(ResetCharmExitFlagNextFrame());
    }

    private IEnumerator ResetCharmExitFlagNextFrame()
    {
        yield return null;
        isExitingCharm = false;
    }

    #endregion

    #region Charming Mode

    //SELECCIONAMOS AL ENEMIGO Y LO ASIGNAMOS COMO HOVERED ENEMY
    private void HandleCharmingMode() 
    {
        if (!isCharming || charmedTarget != null)
        {
            ClearHoveredTarget();
            return;
        }

        if (RaycastForward(charmRange, out RaycastHit hit) && hit.transform.TryGetComponent(out ICharmEnemy enemy))
        {
            if (!enemy.IsCharmed())
            {
                ClearHoveredTarget();
                hoveredTarget = enemy;
                hoveredTarget.SetTargetVisual(true);
                pController.CanInteract(attackInput, InputType.Press, this, ActionType.Default);
            }
        }
        else
            ClearHoveredTarget();
    }

    //COMPROBAMOS SI PODEMOS SEGUIR SELECCIONANDO EL ENEMIGO AL PASAR EL RATON POR ENCIMA
    private void UpdateHoveredTarget()
    {
        if (hoveredTarget == null) return;

        if (hoveredTarget.IsInChaseOrAttack())
            ClearHoveredTarget();
    }

    private void ClearHoveredTarget()
    {
        if (hoveredTarget != null)
        {
            hoveredTarget.SetTargetVisual(false);
            hoveredTarget = null;
            pController.HideInteract(this);
        }
    }

    #endregion

    #region Vision System

    //VISION POWER; ACTIVAMOS LO QUE SE VE A TRAVES DE LA PARED
    private void UpdateVisionTargets() 
    {
        if (!isCharming) return;

        foreach (IVisible vision in pController.GetVisionObjects())
        {
            bool isVisible = pController.GetDistance(vision.GetVisionPosition()) <= visionRange;
            vision.SetVisiblity(isVisible);
        }

        pController.SetUsingVision(true);
    }

    //DESACTIVAMOS LO QUE SE VE A TRAVES
    private void ClearVisionTargets()
    {
        foreach (IVisible vision in pController.GetVisionObjects())
            vision.SetVisiblity(false);

        pController.SetUsingVision(false);
    }

    #endregion

    #region Charmed Logic

    // SI TENEMOS UN ENEMIGO SELECCIONADO, MIRAMOS QUE NO SE SALGA DE LA DISTANCIA MAXIMA O EMPIECE A PERSEGUIRNOS
    private void UpdateCharmedTarget() 
    {
        if (charmedTarget == null) return;

        float distance = Vector3.Distance(transform.position, charmedTarget.GetPosition());

        if (distance > charmRange || !isCharming || charmedTarget.IsInChaseOrAttack())
        {
            ClearCharmedTarget();
            ExitCharmingMode();
        }
    }

    private void ClearCharmedTarget()
    {
        if (charmedTarget != null)
        {
            charmedTarget.ClearIntarectables();
            charmedTarget.SetTargetVisual(false);
            interactables.Clear();
            charmedTarget = null;
            pController.HideInteract(this);
        }
    }

    //ACTIVAMOS LOS INTERACTUABLES CERCA DEL ENEMIGO, PODEMOS SELECCIONARLOS PARA QUE EL ENEMIG VAYA AHI
    private void HandleCharmedInteraction()
    {
        if (charmedTarget == null) return;

        interactables = charmedTarget.ActivateIntarectables();

        if (RaycastForward(Mathf.Infinity, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out IInteractable interactable))
                TryPerformAction(interactable);
            
            else if (hit.transform.TryGetComponent(out IProxy proxy))
                TryPerformAction(proxy.GetObject().lever);
            
            else
                pController.HideInteract(this);
        }
    }

    //APRETAMOS EL INPUT; Y POR LO TANTO MANDAMOS AL ENEMIGO A LA PALANCA Y SALIMOS DEL MODO
    private void TryPerformAction(IInteractable interactable)
    {
        if (interactable.isLocked) return;
        
        pController.CanInteract(attackInput, InputType.Press, this, ActionType.Default);

        if (IsAttackPressed() && interactables.Contains(interactable) && !pController.IsGamePaused)
        {
            charmedTarget.SetCharmedState(interactable);
            
            charmedEnemies.Add(charmedTarget);
            charmedInteractables.Add(interactable);
            
            ClearCharmedTarget();
            ExitCharmingMode();
        }
    }

    //ENSEÑAMOS EL VISUAL EN ROJO SEGUN SI ESTAN CHARMEADOS O NO 
    private void HandleCharmedObjects()
    {
        CheckIfCanRemove();
        
        if (!isCharming)
        {
            TurnOffCharmedObjects();
            return;
        }

        foreach (var enemy in charmedEnemies)
            enemy.SetTargetVisual(true);

        foreach (var interactable in charmedInteractables)
            interactable.SelectObject(true);
    }

    private void CheckIfCanRemove()
    {
        for (int i = charmedEnemies.Count - 1; i >= 0; i--)
        {
            var enemy = charmedEnemies[i];
            if (!enemy.IsCharmed())
            {
                enemy.SetTargetVisual(false);
                charmedEnemies.RemoveAt(i);
            }
        }

        for (int i = charmedInteractables.Count - 1; i >= 0; i--)
        {
            var interactable = charmedInteractables[i];
            if (!interactable.isLocked)
            {
                interactable.SelectObject(false);
                charmedInteractables.RemoveAt(i);
            }
        }
    }
    
    private void TurnOffCharmedObjects()
    {
        foreach (var enemy in charmedEnemies)
            enemy.SetTargetVisual(false);

        foreach (var interactable in charmedInteractables)
            interactable.SelectObject(false);
    }

    #endregion

    #region Helpers

    private bool RaycastForward(float range, out RaycastHit hit)
    {
        return Physics.Raycast(leanParent.position, leanParent.forward, out hit, range, allLayers);
    }

    private void RestartGame()
    {
        ExitCharmingMode();
        TurnOffCharmedObjects();
        checkpointEnemeis.Clear();
        charmedInteractables.Clear();
    }

    private void RestartFromCheckpoint()
    {
        ExitCharmingMode();
        TurnOffCharmedObjects();

        if (checkpointEnemeis.Count != 0)
        {
            for (int i = 0; i < checkpointEnemeis.Count; i++)
            {
                checkpointEnemeis[i].SetCharmedState(checkpointInteractables[i]);
            }
        }
        
        charmedEnemies = new List<ICharmEnemy>(checkpointEnemeis);
        charmedInteractables = new List<IInteractable>(checkpointInteractables);
    }

    private void SetCheckpoint()
    {
        checkpointEnemeis = new List<ICharmEnemy>(charmedEnemies);
        checkpointInteractables = new List<IInteractable>(charmedInteractables);
    }

    #endregion
    
    private void OnEnable()
    {
        charmInput.action.Enable();
        attackInput.action.Enable();
        pController.onRestart += RestartGame;
        pController.onSetCheckpoint += SetCheckpoint;
        pController.onRestartFromCheckpoint += RestartFromCheckpoint;
    }

    private void OnDisable()
    {
        charmInput.action.Disable();
        attackInput.action.Disable();
        pController.onRestart -= RestartGame;
        pController.onSetCheckpoint -= SetCheckpoint;
        pController.onRestartFromCheckpoint -= RestartFromCheckpoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, charmRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
