using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour, IStatue, IVisible
{
    [Tooltip("Punto en el que se produce el tp")]
    [SerializeField] private Transform teleportPoint;

    [SerializeField] private Color visualColor;
    [SerializeField] private Color selectColor;
    [SerializeField] private List<Outline> outlines = new List<Outline>();

    private bool selecting;
    
    void Start()
    {
        GameManager.GetInstance().GetPlayerController().AddVisible(this);
    }
    public void ShowUI(bool show)
    {
        if (show)
        {
            ChangeOutlineMode(true);
            EnableOutline(true);
            selecting = true;
        }
        else
        {
            EnableOutline(false);
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
            ChangeOutlineMode(false);
            EnableOutline(true);
        }
        else
            EnableOutline(false);
    }

    private void ChangeOutlineMode(bool select)
    {
        if (select)
        {
            foreach (Outline script in outlines)
            {
                script.OutlineMode = Outline.Mode.OutlineAll;
                script.OutlineColor = selectColor;
            }
        }
        else
        {
            foreach (Outline script in outlines)
            {
                script.OutlineMode = Outline.Mode.OutlineAndSilhouette;
                script.OutlineColor = visualColor;
            }
        }
    }

    private void EnableOutline(bool active)
    {
        foreach (Outline script in outlines)
            script.enabled = active;
    }

    public Vector3 GetVisionPosition()
    {
        return transform.position;
    }
    
    private void OnDestroy() => GameManager.GetInstance().GetPlayerController().RemoveVisible(this);
}
