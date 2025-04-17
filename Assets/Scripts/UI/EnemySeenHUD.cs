using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySeenHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private int spawnAmount = 5;
    
    private CPoolElement arrowPool;
    private Dictionary<int, ISeeArrow> arrows = new Dictionary<int, ISeeArrow>();

    private void Awake()
    {
        GameManager.GetInstance().SetEnemySeenHUD(this);
        arrowPool = new CPoolElement(spawnAmount, arrowPrefab, transform);
    }

    public void SetNewArrow(Transform target, int enemyID)
    {
        ISeeArrow arrow;
        if (arrows.ContainsKey(enemyID))
            arrow = arrows[enemyID];
        else
        {
            arrow = arrowPool.GetNextElement();
            arrows.Add(enemyID, arrow);
        }
        arrow.SetTarget(target);
        arrow.SetActive(true);
    }

    public void UpdateArrowFillAmount(int enemyID, float fillAmount, float maxSeenCapacity)
    {
        arrows[enemyID].UpdateArrow(fillAmount, maxSeenCapacity);
    }

    public void HideArrow(int enemyID)
    {
        arrows[enemyID].SetActive(false);
        arrows.Remove(enemyID);
    }

    public void ShowExclamation(int enemyID)
    {
        arrows[enemyID].PlayerSeen();
        arrows.Remove(enemyID);
    }
}
