using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyRender : MonoBehaviour
{
    [SerializeField] private GameObject renderGameObject;
    [SerializeField] private Renderer enemyMatRender;
    [SerializeField] private Renderer targetVisual;
    [SerializeField] private Material lockedMat;
    [SerializeField] private GameObject lightSource;
    private CapsuleCollider collider;

    private Material previousMat;

    public void SetRenderer()
    {
        previousMat = targetVisual.material;
        collider = GetComponent<CapsuleCollider>();
    }

    public void ChangeMaterial(Material material)
    {
        enemyMatRender.material = material;
    }

    public void SetTargetVisualActive(bool active)
    {
        if (targetVisual == null) return;
        targetVisual.enabled = active;
    }

    public void SetLockedVisual(bool active)
    {
        if (active)
            targetVisual.material = lockedMat;
        else
            targetVisual.material = previousMat;
    }

    public void SetRenderActive(bool active)
    {
        renderGameObject.SetActive(active);
        collider.enabled = active;
    }
    
    public void SetLight(bool active) => lightSource.SetActive(active);
}
