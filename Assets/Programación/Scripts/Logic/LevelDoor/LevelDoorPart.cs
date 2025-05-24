using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoorPart : MonoBehaviour, IInteractable
{
    public float InteractDistance { get; private set; }
    public bool isLocked { get; set; }
    public bool canInteract { get; set; }

    private LevelDoor door;
    [SerializeField] private Outline outlineScript;

    public void SetDoorPart(LevelDoor door, float interactiDistance, Color selectedColor)
    {
        InteractDistance = interactiDistance;
        canInteract = false;
        outlineScript.OutlineColor = selectedColor;
        this.door = door;
    }

    public void SelectObject(bool select)
    {
        door.SelectObjects(select);
    }

    public void ChangeSelected(bool selected)
    {
        canInteract = selected;
       
        if (selected)
            SetSelectedMat();
        else
            SetDefaultMat();
    }

    private void SetSelectedMat()
    {
        outlineScript.enabled = true;
    }

    private void SetDefaultMat()
    {
        outlineScript.enabled = false;
    }
    public void Interact()
    {
        if (!canInteract) return;
        
        door.ToggleDoor();
    }
    
    public Vector3 GetPosition() => transform.position;
}
