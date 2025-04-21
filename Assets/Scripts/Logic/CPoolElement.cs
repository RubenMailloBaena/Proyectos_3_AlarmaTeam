using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPoolElement
{
    private List<ISeeArrow> pool;
    private int currentElementId = 0;
    private GameObject prefabElement;
    private Transform spawnParent;

    public CPoolElement(int ElementsCount, GameObject PrefabElement, Transform spawnParent)
    {
        pool = new List<ISeeArrow>();
        this.prefabElement = PrefabElement;
        this.spawnParent = spawnParent;

        for (int i = 0; i < ElementsCount; i++)
        {
            InstanciateArrow();
        }
    }

    public ISeeArrow GetNextElement()
    {
        if (AllArrowsInUse())
        {
            return InstanciateArrow();
        }

        ISeeArrow arrow = pool[currentElementId];
        currentElementId++;

        if (currentElementId >= pool.Count)
            currentElementId = 0;

        return arrow;
    }

    private ISeeArrow InstanciateArrow()
    {
        GameObject gameObject = GameObject.Instantiate(prefabElement, spawnParent);
        gameObject.SetActive(false); 
        if (gameObject.TryGetComponent(out ISeeArrow newArrow))
        {
            newArrow.SetActive(false); 
            pool.Add(newArrow);
            return newArrow;
        }
        Debug.LogError("COULDN'T SPAWN NEW ARROW!");
        return null;
    }

    public void HideAllArrows()
    {
        foreach (ISeeArrow arrow in pool)
            arrow.StopAll();
    }

    private bool AllArrowsInUse()
    {
        foreach (var arrow in pool)
        {
            if (!arrow.IsActive) 
                return false; 
        }
        return true; 
    }
}