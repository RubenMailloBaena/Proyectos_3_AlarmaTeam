using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactDistance;
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
    
    private LineRenderer lineRender;
    
    public float InteractDistance => interactDistance;


    private void Start()
    {
        lineRender = GetComponent<LineRenderer>();
        
    }

    public void SelectObject(bool select)
    {
        print("SELECT LEVER: " + select);
    }

    public void Interact()
    {
        foreach (GameObject item in objectsToActivate)
            if (item.TryGetComponent(out IObjects IObject))
                IObject.Interact();
    }
}
    
