using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    float InteractDistance { get; }
    public void SelectObject(bool select);
    public void Interact();
    public Vector3 GetPosition();
}
