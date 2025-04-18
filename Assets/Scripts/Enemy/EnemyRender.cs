using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRender : MonoBehaviour
{
    [SerializeField] private Renderer eRenderer;
    [SerializeField] private Renderer targetVisual;
    [SerializeField] private Material lockedMat;
    private Material previousMat;

    public void SetRenderer()
    {
        previousMat = targetVisual.material;
    }

    public void ChangeMaterial(Material material)
    {
        eRenderer.material = material;
    }

    public void SetTargetVisualActive(bool active)
    {
        targetVisual.enabled = active;
    }

    public void SetLockedVisual(bool active)
    {
        if (active)
            targetVisual.material = lockedMat;
        else
            targetVisual.material = previousMat;
    }
}
