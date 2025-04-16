using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjects
{
    IInteractable lever { get; set; }
    void Interact();
    void ShowInteract(bool interact);
    Vector3 GetCablePosition();
}
