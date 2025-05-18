using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainAdapter : MonoBehaviour, IObjects, IRestartable
{
    [SerializeField] private Transform cablePosition;
    [SerializeField] private List<Curtain> curtains;

    public IInteractable lever { get; set; }
    public Color lockedColor { get; set; }

    private void Start()
    {
        GameManager.GetInstance().AddRestartable(this);
    }

    public void Interact()
    {
        foreach (Curtain curtain in curtains)
            curtain.Interact();
    }

    public void ShowInteract(bool interact, bool locked)
    {
        foreach (Curtain curtain in curtains)
            curtain.ShowInteract(interact, locked, lockedColor);
    }

    public Vector3 GetCablePosition()
    {
        return cablePosition.position;
    }

    public void RestartGame()
    {
        foreach (Curtain curtain in curtains)
            curtain.RestartGame();
    }

    public void RestartFromCheckPoint()
    {
        foreach (Curtain curtain in curtains)
            curtain.RestartFromCheckPoint();
    }

    public void SetCheckPoint()
    {
        foreach (Curtain curtain in curtains)
            curtain.SetCheckPoint();
    }

    private void OnDestroy()
    {
        GameManager.GetInstance().RemoveRestartable(this);
    }
}
