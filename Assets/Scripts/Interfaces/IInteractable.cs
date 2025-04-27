using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    float InteractDistance { get; }
    bool isLocked { get; set; }
    bool canInteract { get; set; }
    public void SelectObject(bool select);
    public void Interact();
    public Vector3 GetPosition();
}
