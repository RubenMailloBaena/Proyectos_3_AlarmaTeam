using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainAdapter : MonoBehaviour, IObjects
{
    [SerializeField] private Transform cablePosition;
    [SerializeField] private List<Curtain> curtains;

    public IInteractable lever { get; set; }
    public Material lockedMaterial { get; set; }

    public void Interact()
    {
        foreach (Curtain curtain in curtains)
            curtain.Interact();
    }

    public void ShowInteract(bool interact, bool locked)
    {
        foreach (Curtain curtain in curtains)
            curtain.ShowInteract(interact, locked, lockedMaterial);
    }
    
    public void SetLocked(bool active)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 GetCablePosition()
    {
        return cablePosition.position;
    }
}
