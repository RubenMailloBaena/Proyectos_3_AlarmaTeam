using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainAdapter : MonoBehaviour, IObjects
{
    [SerializeField] private Transform cablePosition;
    [SerializeField] private List<Curtain> curtains;

    public IInteractable lever { get; set; }

    public void Interact()
    {
        foreach (Curtain curtain in curtains)
            curtain.Interact();
    }

    public void ShowInteract(bool interact)
    {
        foreach (Curtain curtain in curtains)
            curtain.ShowInteract(interact);
    }

    public Vector3 GetCablePosition()
    {
        return cablePosition.position;
    }
}
