using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyRender : MonoBehaviour
{
    [SerializeField] private GameObject renderGameObject;
    [SerializeField] private GameObject lightSource;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color lockedColor;
    [SerializeField] private List<Outline> outlines;
    private bool isLocked = false;

    public void ChangeOutline(bool active)
    {
        Color color = isLocked ? lockedColor : selectedColor;

        foreach (Outline outline in outlines)
        {
            outline.OutlineColor = color;
            outline.OutlineWidth = active ?  3f : 0.0f;
        }
    }

    public void SetLocked(bool locked) => isLocked = locked;

    public void SetRenderActive(bool active)
    {
        renderGameObject.SetActive(active);
    }
    
    public void SetLight(bool active) => lightSource.SetActive(active);
}
