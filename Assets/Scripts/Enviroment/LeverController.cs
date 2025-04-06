using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour, IInteractable, IVisible
{
    [SerializeField] private GameObject selectVisual;
    
    [SerializeField] private float interactDistance;
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
    
    private LineRenderer lineRender;
    private List<IObjects> objects = new List<IObjects>();
    
    public float InteractDistance => interactDistance;

    private void Awake()
    {
        lineRender = GetComponent<LineRenderer>();
        for(int i=0; i < objectsToActivate.Count; i++)
            if (objectsToActivate[i].TryGetComponent(out IObjects item))
                objects.Add(item);

        CalculateLineRenderPositions();
    }

    private void Start()
    {
        GameManager.GetInstance().GetPlayerController().AddVisible(this);
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
    
    public void SelectObject(bool select)
    {
        selectVisual.SetActive(select);
        lineRender.enabled = select;
        foreach (IObjects item in objects)
            item.ShowInteract(select);
    }

    public void Interact()
    {
        foreach (IObjects item in objects)
            item.Interact();
    }

    public void SetVisiblity(bool active)
    {
        selectVisual.SetActive(active);
    }

    public Vector3 GetVisionPosition()
    {
        return transform.position;
    }
}
    
