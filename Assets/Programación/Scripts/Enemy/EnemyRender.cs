using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyRender : MonoBehaviour
{
    private EnemyController eController;
    
    [SerializeField] private GameObject renderGameObject;
    [SerializeField] private GameObject lightSource;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color lockedColor;
    [SerializeField] private List<Outline> outlines;
    [SerializeField] private List<Renderer> bodyRenderParts;
    private bool isLocked = false;
    
    private bool shouldDissolve = false, dissolveEnemie = false;
    private float dissolveValue = 0f;
    public float dissolveSpeed = 0.2f; 
    private List<MaterialPropertyBlock> mpbList = new List<MaterialPropertyBlock>();

    public void SetRender(EnemyController eController)
    {
        this.eController = eController;
        
        foreach (var renderer in bodyRenderParts)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Dissolve", 0f);
            renderer.SetPropertyBlock(mpb);
            mpbList.Add(mpb);
        }
    }
    
    public void ChangeOutline(bool active)
    {
        Color color = isLocked ? lockedColor : selectedColor;

        foreach (Outline outline in outlines)
        {
            outline.OutlineColor = color;
            outline.OutlineWidth = active ?  3f : 0.0f;
        }
    }

    private void Update()
    {
        DissolveEnemy();
    }

    private void DissolveEnemy()
    {
        if (!dissolveEnemie) return;

        dissolveValue = Mathf.MoveTowards(dissolveValue, 1f, Time.deltaTime * dissolveSpeed);

        for (int i = 0; i < bodyRenderParts.Count; i++)
        {
            var renderer = bodyRenderParts[i];
            var mpb = mpbList[i];

            mpb.SetFloat("_Dissolve", dissolveValue);
            renderer.SetPropertyBlock(mpb);
        }
        
        if (dissolveValue >= 1f)
            dissolveEnemie = false; 
    }

    public void SetLocked(bool locked) => isLocked = locked;

    public void DissolveEnemie()
    {
        if(eController.enemyIsDead)
            dissolveEnemie = true;
    } 
    public void ResetDissolve()
    {
        dissolveEnemie = false;
        dissolveValue = 0f; 

        for (int i = 0; i < bodyRenderParts.Count; i++)
        {
            var renderer = bodyRenderParts[i];
            var mpb = mpbList[i];

            mpb.SetFloat("_Dissolve", 0.0f);
            renderer.SetPropertyBlock(mpb);
        }
    }

    public void SetRenderActive(bool active)
    {
        renderGameObject.SetActive(active);
    }
    
    public void SetLight(bool active) => lightSource.SetActive(active);
}
