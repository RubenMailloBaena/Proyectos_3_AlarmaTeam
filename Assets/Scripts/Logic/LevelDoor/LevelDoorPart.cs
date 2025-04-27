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
    private Renderer doorPartRender;
    private Material defaultMat, selectedMat;

    private void Awake()
    {
        doorPartRender = GetComponent<Renderer>();
        defaultMat = doorPartRender.material;
    }

    public void SetDoorPart(LevelDoor door, float interactiDistance, Material selectedMat)
    {
        InteractDistance = interactiDistance;
        canInteract = false;
        this.selectedMat = selectedMat;
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
            doorPartRender.material = selectedMat;
        else
            doorPartRender.material = defaultMat;
    }

    public void Interact()
    {
        if (!canInteract) return;
        
        door.ToggleDoor();
    }
    
    public Vector3 GetPosition() => transform.position;
}
