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
    private Material selectedMat;

    [SerializeField] private List<Renderer> doorRenders;
    private List<Material> defaultMats = new List<Material>();

    private void Awake()
    {
        for (int i = 0; i < doorRenders.Count; i++)
            defaultMats.Add(doorRenders[i].material);
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
            SetSelectedMat();
        else
            SetDefaultMat();
    }

    private void SetSelectedMat()
    {
        for (int i = 0; i < doorRenders.Count; i++)
            doorRenders[i].material = selectedMat;
    }

    private void SetDefaultMat()
    {
        for (int i = 0; i < doorRenders.Count; i++)
            doorRenders[i].material = defaultMats[i];
    }
    public void Interact()
    {
        if (!canInteract) return;
        
        door.ToggleDoor();
    }
    
    public Vector3 GetPosition() => transform.position;
}
