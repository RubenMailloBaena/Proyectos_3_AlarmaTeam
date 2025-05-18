using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Statue : MonoBehaviour, IStatue, IVisible
{
    [Tooltip("Punto en el que se produce el tp")]
    [SerializeField] private Transform teleportPoint;

    [SerializeField] private Color visualColor;
    [SerializeField] private Color selectColor;
    [SerializeField] private Outline outlineScript;

    private bool selecting;
    
    void Start()
    {
        GameManager.GetInstance().GetPlayerController().AddVisible(this);
    }
    public void ShowUI(bool show)
    {
        if (show)
        {
            outlineScript.OutlineMode = Outline.Mode.OutlineAll;
            outlineScript.OutlineColor = selectColor;
            outlineScript.enabled = true;
            selecting = true;
        }
        else
        {
            outlineScript.enabled = false;
            selecting = false;
        }
    }

    public Vector3 GetTPPoint()
    {
        return teleportPoint.position;
    }

    public Quaternion GetTPRotation()
    {
        return Quaternion.LookRotation(teleportPoint.forward);
    }

    public void SetVisiblity(bool active)
    {
        if (selecting) return;
        
        if (active)
        {
            outlineScript.OutlineMode = Outline.Mode.OutlineAndSilhouette;
            outlineScript.OutlineColor = visualColor;
            outlineScript.enabled = true;
        }
        else
            outlineScript.enabled = false;
    }

    public Vector3 GetVisionPosition()
    {
        return transform.position;
    }
    
    private void OnDestroy() => GameManager.GetInstance().GetPlayerController().RemoveVisible(this);
}
