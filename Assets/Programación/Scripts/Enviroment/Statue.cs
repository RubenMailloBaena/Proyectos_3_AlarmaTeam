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

    private void EnableOutline(bool active)
    {
        outlines.RemoveAll(o => o == null);

        foreach (Outline script in outlines)
        {
            if (script != null)     
                script.enabled = active;
        }
    }

    private void ChangeOutlineMode(bool select)
    {
        outlines.RemoveAll(o => o == null);

        Outline.Mode mode = select ? Outline.Mode.OutlineAll : Outline.Mode.OutlineAndSilhouette;
        Color color = select ? selectColor : visualColor;

        foreach (Outline script in outlines)
        {
            if (script != null)
            {
                script.OutlineMode = mode;
                script.OutlineColor = color;
            }
        }
    }


    public Vector3 GetVisionPosition()
    {
        return transform.position;
    }
    
    private void OnDestroy() => GameManager.GetInstance().GetPlayerController().RemoveVisible(this);
}
