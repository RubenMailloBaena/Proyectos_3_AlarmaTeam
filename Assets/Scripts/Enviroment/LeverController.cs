using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour, IInteractable, IVisible
{
    [SerializeField] private Material visualMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material lockedMaterial;
    private Material lineRenderPreviousMat;

    [SerializeField] private Renderer leverRenderer;
    [SerializeField] private Renderer baseRenderer;

    [SerializeField] private float interactDistance;
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
    
    private LineRenderer lineRender;
    private List<IObjects> objects = new List<IObjects>();

    public float InteractDistance => interactDistance;
    public bool isLocked { get; set; }

    private void Awake()
    {
        lineRender = GetComponent<LineRenderer>();
        lineRenderPreviousMat = lineRender.material;
        
        for(int i=0; i < objectsToActivate.Count; i++)
            if (objectsToActivate[i].TryGetComponent(out IObjects item))
            {
                item.lever = this;
                item.lockedMaterial = lockedMaterial;
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
    }
    
    public void SelectObject(bool select)
    {
        SetVisiblity(select);
        lineRender.enabled = select;
        foreach (IObjects item in objects)
            item.ShowInteract(select, isLocked);
    }

    public void Interact()
    {
        foreach (IObjects item in objects)
            item.Interact();
    }

    //VISION
    public void SetVisiblity(bool active)
    {
        if (active)
        {
            if (isLocked)
            {
                ChangeMaterial(lockedMaterial);
                lineRender.material = lockedMaterial;
            }
            else
            {
                ChangeMaterial(visualMaterial);
                lineRender.material = lineRenderPreviousMat;
            }
        }
        else  
            ChangeMaterial(defaultMaterial);
    }

    private void ChangeMaterial(Material mat)
    {
        leverRenderer.material = mat;
        baseRenderer.material = mat;
    }

    public Vector3 GetVisionPosition()
    {
        return transform.position;
    }
    
    public Vector3 GetPosition() => transform.position;
    
}
    
