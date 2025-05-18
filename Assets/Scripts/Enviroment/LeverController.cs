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
    private Material lineRenderMat;

    [SerializeField] private Transform stickTrans;
    [SerializeField] private Outline baseOutline;
    [SerializeField] private Outline stickOutline;

    [Header("Attributes")] 
    [SerializeField] private float animationRotateAngle = 40f;
    [SerializeField] private float animationSpeed = 5;
    [SerializeField] private float interactDistance;
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
    
    private LineRenderer lineRender;
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
        lineRender = GetComponent<LineRenderer>();
        lineRenderMat = lineRender.material;
        
        for(int i=0; i < objectsToActivate.Count; i++)
            if (objectsToActivate[i].TryGetComponent(out IObjects item))
            {
                item.lever = this;
                item.lockedColor = lockedColor;
                objects.Add(item);
            }

        CalculateLineRenderPositions();
    }
    
    private void CalculateLineRenderPositions()
    {
        lineRender.positionCount = objects.Count * 2;

        for (int i = 0; i < lineRender.positionCount; i++)
        {
            if(i%2==0)
                lineRender.SetPosition(i, transform.position);
            else
                lineRender.SetPosition(i, objects[i/2].GetCablePosition());
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

    public void SelectObject(bool select)
    {
        selecting = select;
        SetVisiblity(select);
        lineRender.enabled = select;
        foreach (IObjects item in objects)
            item.ShowInteract(select, isLocked);
    }

    public void Interact()
    {
        PlayAnimation();    
        foreach (IObjects item in objects)
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

    //VISION
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
            TurnOffOutline();
    }

    private void ChangeLineRendererColor(bool locked)
    {
        if (locked)
            lineRenderMat.color = lockedColor;
        else
            lineRenderMat.color = Color.white;
        lineRender.material = lineRenderMat;
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
    

    //CHECKPOINTS
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
    
