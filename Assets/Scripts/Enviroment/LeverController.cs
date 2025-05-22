using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour, IInteractable, IVisible, IRestartable
{
    [Header("References")]
    [SerializeField] private Color visualColor;
    [SerializeField] private Color selectColor;
    [SerializeField] private Color lockedColor;

    [Header("Line Rendering")]
    [Tooltip("Material con la textura de flechas para los cables")]
    [SerializeField] private Material arrowMaterial;
    private List<LineRenderer> lines = new List<LineRenderer>();

    [Header("Lever Parts")]
    [SerializeField] private Transform stickTrans;
    [SerializeField] private Outline baseOutline;
    [SerializeField] private Outline stickOutline;

    [Header("Attributes")]
    [SerializeField] private float animationRotateAngle = 40f;
    [SerializeField] private float animationSpeed = 5f;
    [SerializeField] private float interactDistance;
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    private List<IObjects> objects = new List<IObjects>();

    public float InteractDistance => interactDistance;
    public bool isLocked { get; set; }
    public bool canInteract { get; set; }

    private bool wasLocked;
    private bool isMoving, selecting;
    private float targetZ;

    private void Awake()
    {
        canInteract = true;

        // 1) Recogemos las interfaces IObjects
        foreach (var go in objectsToActivate)
        {
            if (go.TryGetComponent<IObjects>(out var item))
            {
                item.lever = this;
                item.lockedColor = lockedColor;
                objects.Add(item);
            }
        }
        // 2) Creamos un LineRenderer por cada objeto
        foreach (var obj in objects)
        {
            // Creamos un GameObject para el cable
            var cableGO = new GameObject("CableTo_" + obj.GetCablePosition());
            cableGO.transform.SetParent(transform, worldPositionStays: true);

            // Añadimos y configuramos el LineRenderer
            var lr = cableGO.AddComponent<LineRenderer>();
            lr.material = arrowMaterial;
            lr.widthMultiplier = 0.308f;
            lr.positionCount = 2;
            lr.textureMode = LineTextureMode.Tile;
            

            // Posicionamos los extremos
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, obj.GetCablePosition());

            lr.enabled = false;  // oculto inicialmente
            lines.Add(lr);
        }
    }

    private void Start()
    {
        GameManager.GetInstance().GetPlayerController().AddVisible(this);
        GameManager.GetInstance().AddInteractable(this);
        GameManager.GetInstance().AddRestartable(this);
        targetZ = stickTrans.localEulerAngles.z;
    }

    private void Update()
    {
        UpdateAnimation();
    }

    // IVisible
    public void SelectObject(bool select)
    {
        selecting = select;
        SetVisiblity(select);

        // mostramos/ocultamos todas las líneas
        foreach (var lr in lines)
            lr.enabled = select;

        // notificamos a cada objeto
        foreach (var item in objects)
            item.ShowInteract(select, isLocked);
    }

    // IInteractable
    public void Interact()
    {
        AudioManager.Instance.HandlePlaySound3D(
            "event:/Ambiente/ambiente_palanca",
            transform.position
        );
        PlayAnimation();
        foreach (var item in objects)
            item.Interact();
    }

    private void PlayAnimation()
    {
        isMoving = !isMoving;
        targetZ = isMoving ? -animationRotateAngle : animationRotateAngle;
    }

    private void UpdateAnimation()
    {
        float currentZ = NormalizeAngle(stickTrans.localEulerAngles.z);
        float newZ = Mathf.Lerp(currentZ, targetZ, animationSpeed * Time.deltaTime);
        stickTrans.localEulerAngles = new Vector3(
            stickTrans.localEulerAngles.x,
            stickTrans.localEulerAngles.y,
            newZ
        );
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    // Visualización / contornos
    public void SetVisiblity(bool active)
    {
        if (active)
        {
            ChangeLineRendererColor(false);

            if (isLocked)
            {
                ChangeMaterial(lockedColor, true);
                ChangeLineRendererColor(true);
            }
            else
            {
                if (selecting)
                    ChangeMaterial(selectColor, false);
                else
                    ChangeMaterial(visualColor, true);
            }
        }
        else
        {
            TurnOffOutline();
        }
    }

    private void ChangeLineRendererColor(bool locked)
    {
        Color c = locked ? lockedColor : Color.white;
        foreach (var lr in lines)
            lr.material.color = c;
    }

    private void ChangeMaterial(Color color, bool isVision)
    {
        baseOutline.enabled = true;
        stickOutline.enabled = true;
        baseOutline.OutlineMode = Outline.Mode.OutlineAll;
        stickOutline.OutlineMode = Outline.Mode.OutlineAll;

        if (isVision)
        {
            baseOutline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
            stickOutline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        }

        baseOutline.OutlineColor = color;
        stickOutline.OutlineColor = color;
    }

    private void TurnOffOutline()
    {
        baseOutline.enabled = false;
        stickOutline.enabled = false;
    }

    public Vector3 GetVisionPosition() => transform.position;

    // CHECKPOINTS
    public void RestartGame()
    {
        isLocked = false;
        wasLocked = false;
        SelectObject(false);
    }

    public void RestartFromCheckPoint()
    {
        isLocked = wasLocked;
    }

    public void SetCheckPoint()
    {
        wasLocked = isLocked;
    }

    private void OnDestroy()
    {
        GameManager.GetInstance().GetPlayerController().RemoveVisible(this);
        GameManager.GetInstance().RemoveInteractable(this);
        GameManager.GetInstance().RemoveRestartable(this);
    }

    public Vector3 GetPosition() => transform.position;
}
