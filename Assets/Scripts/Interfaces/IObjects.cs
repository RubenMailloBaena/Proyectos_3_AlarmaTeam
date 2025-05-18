using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjects
{
    IInteractable lever { get; set; }
    Color lockedColor { get; set; }
    void Interact();
    void ShowInteract(bool interact, bool locked);
    Vector3 GetCablePosition();
}
